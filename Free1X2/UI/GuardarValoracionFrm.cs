// created on 27/07/2004 at 18:30
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
// Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.

using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using Free1X2.Utils ;

using Free1X2.UI.Modern.Theming;
namespace Free1X2.UI
{
	/// <summary>
	/// Summary description for GuardarValoracion.
	/// </summary>
	public class GuardarValoracionFrm : System.Windows.Forms.Form
	{
		private System.Windows.Forms.GroupBox grFormato;
		private System.Windows.Forms.RadioButton rb3ValoresPorFila;
		private System.Windows.Forms.RadioButton rb1ValorPorFila;
		private System.Windows.Forms.GroupBox grSeparador;
		private System.Windows.Forms.RadioButton rbTabulador;
		private System.Windows.Forms.RadioButton rbComa;
		private System.Windows.Forms.RadioButton rbEspacio;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		public char separador;
		public short formato3ValoresPorFila;
		private System.Windows.Forms.Button btAceptar;
		private System.Windows.Forms.Button btCancelar;
		public string fichero;
		private double[] _valores1;
		private double[] _valoresX;
		private double[] _valores2;
		private double[,] _valores1x2;
		private System.Windows.Forms.RadioButton rb42ValoresPorFila;
		private bool EsMatrizDe2Dimensiones;

		public GuardarValoracionFrm(double[] valores1, double[] valoresX, double[] valores2)
		{
			InitializeComponent();

			_valores1=valores1;
			_valoresX=valoresX;
			_valores2=valores2;
			EsMatrizDe2Dimensiones=false;
            FormulariosHelper fH = new FormulariosHelper();
            fH.Traducir(this);
		}
        protected override void OnLoad(System.EventArgs e)
        {
            base.OnLoad(e);
            ModernTheme.ApplyToForm(this);
        }


		public GuardarValoracionFrm(double[,] p)
		{
			InitializeComponent();
			EsMatrizDe2Dimensiones=true;
			_valores1x2=p;
            FormulariosHelper fH = new FormulariosHelper();
            fH.Traducir(this);
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GuardarValoracionFrm));
            this.grFormato = new System.Windows.Forms.GroupBox();
            this.rb42ValoresPorFila = new System.Windows.Forms.RadioButton();
            this.grSeparador = new System.Windows.Forms.GroupBox();
            this.rbEspacio = new System.Windows.Forms.RadioButton();
            this.rbComa = new System.Windows.Forms.RadioButton();
            this.rbTabulador = new System.Windows.Forms.RadioButton();
            this.rb1ValorPorFila = new System.Windows.Forms.RadioButton();
            this.rb3ValoresPorFila = new System.Windows.Forms.RadioButton();
            this.btAceptar = new System.Windows.Forms.Button();
            this.btCancelar = new System.Windows.Forms.Button();
            this.grFormato.SuspendLayout();
            this.grSeparador.SuspendLayout();
            this.SuspendLayout();
            // 
            // grFormato
            // 
            this.grFormato.Controls.Add(this.rb42ValoresPorFila);
            this.grFormato.Controls.Add(this.grSeparador);
            this.grFormato.Controls.Add(this.rb1ValorPorFila);
            this.grFormato.Controls.Add(this.rb3ValoresPorFila);
            this.grFormato.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grFormato.ForeColor = System.Drawing.Color.Maroon;
            this.grFormato.Location = new System.Drawing.Point(17, 28);
            this.grFormato.Name = "grFormato";
            this.grFormato.Size = new System.Drawing.Size(384, 156);
            this.grFormato.TabIndex = 0;
            this.grFormato.TabStop = false;
            this.grFormato.Text = "Formato del fichero";
            // 
            // rb42ValoresPorFila
            // 
            this.rb42ValoresPorFila.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rb42ValoresPorFila.ForeColor = System.Drawing.Color.Black;
            this.rb42ValoresPorFila.Location = new System.Drawing.Point(20, 64);
            this.rb42ValoresPorFila.Name = "rb42ValoresPorFila";
            this.rb42ValoresPorFila.Size = new System.Drawing.Size(172, 16);
            this.rb42ValoresPorFila.TabIndex = 3;
            this.rb42ValoresPorFila.Text = "42 valores en una línea";
            // 
            // grSeparador
            // 
            this.grSeparador.Controls.Add(this.rbEspacio);
            this.grSeparador.Controls.Add(this.rbComa);
            this.grSeparador.Controls.Add(this.rbTabulador);
            this.grSeparador.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grSeparador.ForeColor = System.Drawing.Color.Maroon;
            this.grSeparador.Location = new System.Drawing.Point(207, 24);
            this.grSeparador.Name = "grSeparador";
            this.grSeparador.Size = new System.Drawing.Size(152, 104);
            this.grSeparador.TabIndex = 2;
            this.grSeparador.TabStop = false;
            this.grSeparador.Text = "Separador de campos";
            // 
            // rbEspacio
            // 
            this.rbEspacio.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.rbEspacio.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rbEspacio.ForeColor = System.Drawing.Color.Black;
            this.rbEspacio.Location = new System.Drawing.Point(8, 67);
            this.rbEspacio.Name = "rbEspacio";
            this.rbEspacio.Size = new System.Drawing.Size(136, 24);
            this.rbEspacio.TabIndex = 2;
            this.rbEspacio.Text = "[Espacio en blanco]";
            // 
            // rbComa
            // 
            this.rbComa.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.rbComa.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rbComa.ForeColor = System.Drawing.Color.Black;
            this.rbComa.Location = new System.Drawing.Point(8, 43);
            this.rbComa.Name = "rbComa";
            this.rbComa.Size = new System.Drawing.Size(136, 24);
            this.rbComa.TabIndex = 1;
            this.rbComa.Text = "[Coma]";
            // 
            // rbTabulador
            // 
            this.rbTabulador.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.rbTabulador.Checked = true;
            this.rbTabulador.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rbTabulador.ForeColor = System.Drawing.Color.Black;
            this.rbTabulador.Location = new System.Drawing.Point(8, 19);
            this.rbTabulador.Name = "rbTabulador";
            this.rbTabulador.Size = new System.Drawing.Size(136, 24);
            this.rbTabulador.TabIndex = 0;
            this.rbTabulador.TabStop = true;
            this.rbTabulador.Text = "[Tabulador]";
            // 
            // rb1ValorPorFila
            // 
            this.rb1ValorPorFila.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rb1ValorPorFila.ForeColor = System.Drawing.Color.Black;
            this.rb1ValorPorFila.Location = new System.Drawing.Point(20, 104);
            this.rb1ValorPorFila.Name = "rb1ValorPorFila";
            this.rb1ValorPorFila.Size = new System.Drawing.Size(172, 16);
            this.rb1ValorPorFila.TabIndex = 1;
            this.rb1ValorPorFila.Text = "1 valor por  línea";
            // 
            // rb3ValoresPorFila
            // 
            this.rb3ValoresPorFila.Checked = true;
            this.rb3ValoresPorFila.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rb3ValoresPorFila.ForeColor = System.Drawing.Color.Black;
            this.rb3ValoresPorFila.Location = new System.Drawing.Point(20, 24);
            this.rb3ValoresPorFila.Name = "rb3ValoresPorFila";
            this.rb3ValoresPorFila.Size = new System.Drawing.Size(172, 16);
            this.rb3ValoresPorFila.TabIndex = 0;
            this.rb3ValoresPorFila.TabStop = true;
            this.rb3ValoresPorFila.Text = "3 valores por línea";
            // 
            // btAceptar
            // 
            this.btAceptar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btAceptar.BackColor = System.Drawing.Color.DarkSalmon;
            this.btAceptar.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btAceptar.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btAceptar.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btAceptar.Image = ((System.Drawing.Image)(resources.GetObject("btAceptar.Image")));
            this.btAceptar.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btAceptar.Location = new System.Drawing.Point(215, 190);
            this.btAceptar.Name = "btAceptar";
            this.btAceptar.Size = new System.Drawing.Size(100, 32);
            this.btAceptar.TabIndex = 4;
            this.btAceptar.Text = "&Aceptar";
            this.btAceptar.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btAceptar.UseVisualStyleBackColor = false;
            this.btAceptar.Click += new System.EventHandler(this.btAceptar_Click);
            // 
            // btCancelar
            // 
            this.btCancelar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btCancelar.BackColor = System.Drawing.Color.DarkSalmon;
            this.btCancelar.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btCancelar.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btCancelar.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btCancelar.Image = ((System.Drawing.Image)(resources.GetObject("btCancelar.Image")));
            this.btCancelar.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btCancelar.Location = new System.Drawing.Point(103, 190);
            this.btCancelar.Name = "btCancelar";
            this.btCancelar.Size = new System.Drawing.Size(100, 32);
            this.btCancelar.TabIndex = 5;
            this.btCancelar.Text = "&Cancelar";
            this.btCancelar.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btCancelar.UseVisualStyleBackColor = false;
            this.btCancelar.Click += new System.EventHandler(this.btCancelar_Click);
            // 
            // GuardarValoracionFrm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.Color.Bisque;
            this.ClientSize = new System.Drawing.Size(418, 246);
            this.Controls.Add(this.btCancelar);
            this.Controls.Add(this.btAceptar);
            this.Controls.Add(this.grFormato);
            this.MinimumSize = new System.Drawing.Size(352, 280);
            this.Name = "GuardarValoracionFrm";
            this.Text = "Guardar Valoración";
            this.Load += new System.EventHandler(this.GuardarValoracionFrm_Load);
            this.grFormato.ResumeLayout(false);
            this.grSeparador.ResumeLayout(false);
            this.ResumeLayout(false);

		}
		#endregion

		private void GuardarValoracionFrm_Load(object sender, System.EventArgs e)
		{
		
		}

		private void btCancelar_Click(object sender, System.EventArgs e)
		{
			this.Close();
		}

		private void btAceptar_Click(object sender, System.EventArgs e)
		{
			if(rb3ValoresPorFila.Checked || rb42ValoresPorFila.Checked )
			{
				formato3ValoresPorFila=3;
				if(rb42ValoresPorFila.Checked)formato3ValoresPorFila=42;
				if(rbComa.Checked  ) separador =',';
				if(rbEspacio.Checked  ) separador =' ';
				if(rbTabulador.Checked  ) separador = (char) 9;
			}
			else
			{
				formato3ValoresPorFila=1;
			}

			string archivoSalida;
			SaveFileDialog abreFiltroDialog = new SaveFileDialog();
			abreFiltroDialog.InitialDirectory = "Combinaciones\\" ;
			abreFiltroDialog.Filter = "Valoraciones(*.txt)|*.txt|Todos los archivos (*.*)|*.*" ;
			if(abreFiltroDialog.ShowDialog () == DialogResult.OK)
			{		    	
				archivoSalida = abreFiltroDialog.FileName;		    	
				Porcentajes Pct = new Porcentajes();
				if(EsMatrizDe2Dimensiones)
				{
					Pct.GuardarValoraciones (archivoSalida,formato3ValoresPorFila,separador, _valores1x2);
				}
				else
				{
					Pct.GuardarValoraciones (archivoSalida,formato3ValoresPorFila,separador, _valores1, _valoresX, _valores2);
				}
			}
			this.Close();
		}

	}
}
