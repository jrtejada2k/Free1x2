// Free1X2 : Programa de quinielas "libre"
// Created 11-03-05 at 18:06 
// Copyright (C) 2004 Joan Duatis - duatis@coac.net
// 
// This program is free software; you can redistribute it and/or
// modify it under the terms of the GNU General Public License
// as published by the Free Software Foundation; either version 2
// of the License, or (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using Free1X2.Utils;

using Free1X2.UI.Modern.Theming;
namespace Free1X2.UI
{
	/// <summary>
	/// Summary description for DialogoSeleccionBancoPruebasFrm.
	/// </summary>
	public class DialogoSeleccionBancoPruebasFrm : System.Windows.Forms.Form
	{
		private System.Windows.Forms.DataGrid dataGrid1;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		public ArrayList ValoresMinimosyMaximos=null;
		public bool Cancelado = false;
		private System.Windows.Forms.Button btCancelar;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.Label label1;
	
		private System.ComponentModel.Container components = null;

		public DialogoSeleccionBancoPruebasFrm(double [,] pMinMax)
		{
			InitializeComponent();
			InicializaGrid();


			ValoresMinimosyMaximos= new ArrayList();
			ValoresMinimosyMaximos.Add (new TablaDeSeleccion ("Nº de veces 14 aciertos",pMinMax[0,0],pMinMax[0,1])); 
			ValoresMinimosyMaximos.Add ( new TablaDeSeleccion ("Nº de veces 13 aciertos",pMinMax[1,0],pMinMax[1,1]));
			ValoresMinimosyMaximos.Add ( new TablaDeSeleccion ("Nº de veces 12 aciertos",pMinMax[2,0],pMinMax[2,1]));
			ValoresMinimosyMaximos.Add ( new TablaDeSeleccion ("Nº de veces 11 aciertos",pMinMax[3,0],pMinMax[3,1]));
			ValoresMinimosyMaximos.Add ( new TablaDeSeleccion ("Nº de veces 10 aciertos",pMinMax[4,0],pMinMax[4,1]));
			ValoresMinimosyMaximos.Add ( new TablaDeSeleccion ("Nº de veces con premio",pMinMax[5,0],pMinMax[5,1]));
			ValoresMinimosyMaximos.Add ( new TablaDeSeleccion ("Premio total de 14",pMinMax[6,0],pMinMax[6,1]));
			ValoresMinimosyMaximos.Add ( new TablaDeSeleccion ("Premio total de 13",pMinMax[7,0],pMinMax[7,1]));
			ValoresMinimosyMaximos.Add ( new TablaDeSeleccion ("Premio total de 12",pMinMax[8,0],pMinMax[8,1]));
			ValoresMinimosyMaximos.Add ( new TablaDeSeleccion ("Premio total de 11",pMinMax[9,0],pMinMax[9,1]));
			ValoresMinimosyMaximos.Add ( new TablaDeSeleccion ("Premio total de 10",pMinMax[10,0],pMinMax[10,1]));
			ValoresMinimosyMaximos.Add ( new TablaDeSeleccion ("Premio acumulado",pMinMax[11,0],pMinMax[11,1]));
			ValoresMinimosyMaximos.Add ( new TablaDeSeleccion ("Premio medio de 14",pMinMax[12,0],pMinMax[12,1]));
			ValoresMinimosyMaximos.Add ( new TablaDeSeleccion ("Premio medio de 13",pMinMax[13,0],pMinMax[13,1]));
			ValoresMinimosyMaximos.Add ( new TablaDeSeleccion ("Premio medio de 12",pMinMax[14,0],pMinMax[14,1]));
			ValoresMinimosyMaximos.Add ( new TablaDeSeleccion ("Premio medio de 11",pMinMax[15,0],pMinMax[15,1]));
			ValoresMinimosyMaximos.Add ( new TablaDeSeleccion ("Premio medio de 10",pMinMax[16,0],pMinMax[16,1]));
			ValoresMinimosyMaximos.Add ( new TablaDeSeleccion ("% de recuperación ",pMinMax[17,0],pMinMax[17,1]));
			dataGrid1.DataSource = ValoresMinimosyMaximos;

            FormulariosHelper fH = new FormulariosHelper();
            fH.Traducir(this);
		}
        protected override void OnLoad(System.EventArgs e)
        {
            base.OnLoad(e);
            ModernTheme.ApplyToForm(this);
        }


		/// <summary>
		/// Clean up any resources being used.
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

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DialogoSeleccionBancoPruebasFrm));
            this.dataGrid1 = new System.Windows.Forms.DataGrid();
            this.btCancelar = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dataGrid1)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGrid1
            // 
            this.dataGrid1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.dataGrid1.CaptionVisible = false;
            this.dataGrid1.DataMember = "";
            this.dataGrid1.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.dataGrid1.Location = new System.Drawing.Point(8, 82);
            this.dataGrid1.Name = "dataGrid1";
            this.dataGrid1.RowHeadersVisible = false;
            this.dataGrid1.Size = new System.Drawing.Size(360, 361);
            this.dataGrid1.TabIndex = 0;
            // 
            // btCancelar
            // 
            this.btCancelar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btCancelar.BackColor = System.Drawing.Color.DarkSalmon;
            this.btCancelar.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btCancelar.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btCancelar.Image = ((System.Drawing.Image)(resources.GetObject("btCancelar.Image")));
            this.btCancelar.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btCancelar.Location = new System.Drawing.Point(376, 403);
            this.btCancelar.Name = "btCancelar";
            this.btCancelar.Size = new System.Drawing.Size(88, 32);
            this.btCancelar.TabIndex = 1;
            this.btCancelar.Text = "&Cancelar";
            this.btCancelar.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btCancelar.UseVisualStyleBackColor = false;
            this.btCancelar.Click += new System.EventHandler(this.btCancelar_Click);
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.BackColor = System.Drawing.Color.DarkSalmon;
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.button1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.Image = ((System.Drawing.Image)(resources.GetObject("button1.Image")));
            this.button1.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button1.Location = new System.Drawing.Point(376, 363);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(88, 32);
            this.button1.TabIndex = 2;
            this.button1.Text = "&Aceptar";
            this.button1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.Maroon;
            this.label1.Location = new System.Drawing.Point(8, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(360, 71);
            this.label1.TabIndex = 3;
            this.label1.Text = "Se seleccionaran las columnas comprendidas entre los valores mínimo y máximo de l" +
                "os conceptos seleccionados";
            // 
            // DialogoSeleccionBancoPruebasFrm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.Color.Bisque;
            this.ClientSize = new System.Drawing.Size(472, 449);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.btCancelar);
            this.Controls.Add(this.dataGrid1);
            this.Name = "DialogoSeleccionBancoPruebasFrm";
            this.Text = "Selección de columnas";
            ((System.ComponentModel.ISupportInitialize)(this.dataGrid1)).EndInit();
            this.ResumeLayout(false);

		}
		#endregion

		private void btCancelar_Click(object sender, System.EventArgs e)
		{
			Cancelado=true;
			this.Close ();
		}

		private void button1_Click(object sender, System.EventArgs e)
		{
			Cancelado=false;
			this.Close ();
		}
		protected void InicializaGrid()
		{			
			DataGridTableStyle tableStyle = new DataGridTableStyle();
			tableStyle.MappingName = "ArrayList";
			tableStyle.ColumnHeadersVisible = true;
			
			// Crear Columnas 
			// MappingName tiene que ser igual a cada una de las "properties"
			// de la clase Combinacion.
            
			DataGridTextBoxColumn cs = null;
			//		Concepto
			cs = new DataGridTextBoxColumn();
			cs.MappingName = "Concepto";
			cs.HeaderText = "Concepto";
			cs.Width = 130;
			tableStyle.GridColumnStyles.Add(cs);

			//		Checked
			DataGridBoolColumn cb = null; // columna de tipo booleano
			cb = new DataGridBoolColumn ();
			cb.MappingName = "Checked";
			cb.HeaderText = "";
			cb.Width = 30;
			tableStyle.GridColumnStyles.Add(cb);

			//		Minimo
			cs = new DataGridTextBoxColumn();
			cs.MappingName = "Minimo";
			cs.HeaderText = "Mínimo";
			cs.Width = 70;
			tableStyle.GridColumnStyles.Add(cs);
	
			//       Maximo
			cs = new DataGridTextBoxColumn();
			cs.MappingName = "Maximo";
			cs.HeaderText = "Máximo";
			cs.Width = 70;
			tableStyle.GridColumnStyles.Add(cs);

			dataGrid1.TableStyles.Add(tableStyle);			
		}
		protected void GridDataBind()
		{
			dataGrid1.DataSource =null;
			dataGrid1.DataSource =ValoresMinimosyMaximos;
			dataGrid1.Refresh ();
		}

	}
}
