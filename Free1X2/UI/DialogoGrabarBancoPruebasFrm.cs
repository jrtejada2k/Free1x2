// created on 25/02/2005 at 23:15
// Free1X2 : Programa de quinielas "libre"
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
// Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System;

using Free1X2.UI.Modern.Theming;
namespace Free1X2.UI
{
	/// <summary>
	/// Summary description for DialogoGrabarBancoPruebasFrm.
	/// </summary>
	public class DialogoGrabarBancoPruebasFrm : System.Windows.Forms.Form
	{

		private System.Windows.Forms.Button btGrabar;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox txNumColumns;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox txColumnaFinal;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox txColumnaInicial;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		public int FilaInicial;
		public int FilaFinal;
		public int NumMaxColumnas;
		public bool Cancelado =false;
		public bool SoloSeleccionadas=true;
		private System.Windows.Forms.Button btCancelar;
		protected System.Windows.Forms.CheckBox chkSoloSeleccionadas;
		private System.ComponentModel.Container components = null;

		public DialogoGrabarBancoPruebasFrm(int c1, int c2, int c)
		{
			InitializeComponent();
			txColumnaInicial.Text =c1.ToString ();
			txColumnaFinal.Text =c2.ToString ();
			txNumColumns.Text =c.ToString();
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DialogoGrabarBancoPruebasFrm));
            this.btGrabar = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.txNumColumns = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txColumnaFinal = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txColumnaInicial = new System.Windows.Forms.TextBox();
            this.btCancelar = new System.Windows.Forms.Button();
            this.chkSoloSeleccionadas = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // btGrabar
            // 
            this.btGrabar.BackColor = System.Drawing.Color.DarkSalmon;
            this.btGrabar.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btGrabar.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btGrabar.Image = ((System.Drawing.Image)(resources.GetObject("btGrabar.Image")));
            this.btGrabar.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btGrabar.Location = new System.Drawing.Point(16, 123);
            this.btGrabar.Name = "btGrabar";
            this.btGrabar.Size = new System.Drawing.Size(100, 32);
            this.btGrabar.TabIndex = 15;
            this.btGrabar.Text = "Aceptar";
            this.btGrabar.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btGrabar.UseVisualStyleBackColor = false;
            this.btGrabar.Click += new System.EventHandler(this.btGrabar_Click);
            // 
            // label3
            // 
            this.label3.BackColor = System.Drawing.Color.LemonChiffon;
            this.label3.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(13, 53);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(148, 21);
            this.label3.TabIndex = 26;
            this.label3.Text = "Nº máximo de apuestas";
            // 
            // txNumColumns
            // 
            this.txNumColumns.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txNumColumns.Location = new System.Drawing.Point(164, 53);
            this.txNumColumns.Name = "txNumColumns";
            this.txNumColumns.Size = new System.Drawing.Size(64, 21);
            this.txNumColumns.TabIndex = 25;
            this.txNumColumns.Text = "0";
            // 
            // label2
            // 
            this.label2.BackColor = System.Drawing.Color.LemonChiffon;
            this.label2.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(13, 31);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(148, 21);
            this.label2.TabIndex = 24;
            this.label2.Text = "Fila de la apuesta final";
            // 
            // txColumnaFinal
            // 
            this.txColumnaFinal.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txColumnaFinal.Location = new System.Drawing.Point(164, 31);
            this.txColumnaFinal.Name = "txColumnaFinal";
            this.txColumnaFinal.Size = new System.Drawing.Size(64, 21);
            this.txColumnaFinal.TabIndex = 23;
            this.txColumnaFinal.Text = "0";
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.Color.LemonChiffon;
            this.label1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(13, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(148, 21);
            this.label1.TabIndex = 22;
            this.label1.Text = "Fila de la apuesta inicial";
            // 
            // txColumnaInicial
            // 
            this.txColumnaInicial.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txColumnaInicial.Location = new System.Drawing.Point(164, 9);
            this.txColumnaInicial.Name = "txColumnaInicial";
            this.txColumnaInicial.Size = new System.Drawing.Size(64, 21);
            this.txColumnaInicial.TabIndex = 21;
            this.txColumnaInicial.Text = "0";
            // 
            // btCancelar
            // 
            this.btCancelar.BackColor = System.Drawing.Color.DarkSalmon;
            this.btCancelar.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btCancelar.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btCancelar.Image = ((System.Drawing.Image)(resources.GetObject("btCancelar.Image")));
            this.btCancelar.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btCancelar.Location = new System.Drawing.Point(120, 123);
            this.btCancelar.Name = "btCancelar";
            this.btCancelar.Size = new System.Drawing.Size(100, 32);
            this.btCancelar.TabIndex = 27;
            this.btCancelar.Text = "&Cancelar";
            this.btCancelar.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btCancelar.UseVisualStyleBackColor = false;
            this.btCancelar.Click += new System.EventHandler(this.btCancelar_Click);
            // 
            // chkSoloSeleccionadas
            // 
            this.chkSoloSeleccionadas.Checked = true;
            this.chkSoloSeleccionadas.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkSoloSeleccionadas.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkSoloSeleccionadas.Location = new System.Drawing.Point(22, 92);
            this.chkSoloSeleccionadas.Name = "chkSoloSeleccionadas";
            this.chkSoloSeleccionadas.Size = new System.Drawing.Size(196, 16);
            this.chkSoloSeleccionadas.TabIndex = 28;
            this.chkSoloSeleccionadas.Text = "sólo si estan seleccionadas";
            // 
            // DialogoGrabarBancoPruebasFrm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.Color.Bisque;
            this.ClientSize = new System.Drawing.Size(240, 165);
            this.Controls.Add(this.chkSoloSeleccionadas);
            this.Controls.Add(this.btCancelar);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txNumColumns);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txColumnaFinal);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txColumnaInicial);
            this.Controls.Add(this.btGrabar);
            this.Name = "DialogoGrabarBancoPruebasFrm";
            this.Text = "Grabar apuestas";
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		private void btCancelar_Click(object sender, System.EventArgs e)
		{
			Cancelado = true;
			this.Close ();
		}

		private void btGrabar_Click(object sender, System.EventArgs e)
		{
			FilaInicial = Convert.ToInt32( txColumnaInicial.Text);
			FilaFinal = Convert.ToInt32 ( txColumnaFinal.Text);
			NumMaxColumnas  = Convert.ToInt32( txNumColumns.Text);
			Cancelado = false;
			SoloSeleccionadas=chkSoloSeleccionadas.Checked;
			this.Close ();
		}

	}
}
