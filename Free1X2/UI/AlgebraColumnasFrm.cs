// created on 02/12/2003 at 21:01
// Free1X2 : Programa de quinielas "libre"
// Copyright (C) 2003 Luis Fernandez - luifer@onetel.net.uk
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
using System.Windows.Forms;
using System.IO;

using Free1X2;
using Free1X2.EntradaSalida;
using Free1X2.Utils;

namespace Free1X2.UI
{
	public class AlgebraColumnasFrm : System.Windows.Forms.Form
	{
		private System.Windows.Forms.RadioButton radOption4;
		private System.Windows.Forms.GroupBox groupBox;
		private System.Windows.Forms.Label lblResultado;
		private System.Windows.Forms.Button btnSelComb1;
		private System.Windows.Forms.Button btnSelComb2;
		private System.Windows.Forms.Label lblCombFinal;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.Button btnCalcular;
		private System.Windows.Forms.Label lblComb1;
		private System.Windows.Forms.Label lblComb2;
		private System.Windows.Forms.Button btnSelCombFinal;
		private System.Windows.Forms.Button btnCancelar;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label;
		private System.Windows.Forms.RadioButton radOption1;
		private System.Windows.Forms.RadioButton radOption3;
		private System.Windows.Forms.RadioButton radOption2;
		
		private string archivoCols1 = "";
		private string archivoCols2 = "";
		private string archivoColsFinal = "";
		private System.Windows.Forms.Label lblFiltro1;
		private System.Windows.Forms.Label lblFiltro2;
        private bool salidaBinaria = false;

        private int noSignos1, noSignos2;
        private Label lblFantasma;
		
		private SumadorCombinaciones sumador;
		
		public AlgebraColumnasFrm()
		{
			InitializeComponent();
			//sumador = new SumadorCombinaciones(salidaBinaria);
            FormulariosHelper fH = new FormulariosHelper();
            fH.Traducir(this);
		}
		
		protected void Calcula()
		{
			AlgebraCombTipo tipoCalculo = AlgebraCombTipo.EliminaRepetidas;
			
			if( radOption1.Checked )
			{
				tipoCalculo = AlgebraCombTipo.EliminaRepetidas;
			}
			else if( radOption2.Checked )
			{
				tipoCalculo = AlgebraCombTipo.SumaEliminaRepetidas;
			}
			else if( radOption3.Checked )
			{
				tipoCalculo = AlgebraCombTipo.SumaSoloComunes;
			}
			else if( radOption4.Checked )
			{
				tipoCalculo = AlgebraCombTipo.RestaSegunda;
			}

            if (SonDatosEntradaValidos(tipoCalculo))
            {
                sumador = new SumadorCombinaciones(this.noSignos2);
                sumador.ArchivoCols1 = archivoCols1;
                sumador.ArchivoCols2 = archivoCols2;
                sumador.ArchivoColsFinal = archivoColsFinal;

                lblResultado.Text = "";
                btnCalcular.Enabled = false;
                sumador.Calcula(tipoCalculo);

                lblResultado.Text = sumador.MensajeFinCalculo;
                btnCalcular.Enabled = true;


            }
            else
            {
                lblResultado.Text = "No ha seleccionado los archivos";
            }
		}
		
		protected bool SonDatosEntradaValidos( AlgebraCombTipo tipoCalculo )
		{
			bool esValido = true;
			
			switch( tipoCalculo )
			{
				case AlgebraCombTipo.EliminaRepetidas:
					
					if(archivoCols1 == "" || archivoColsFinal == "")
					{
						esValido = false;
					}					
					
					break;
				case AlgebraCombTipo.SumaEliminaRepetidas:
					
					if(archivoCols1 == "" || archivoCols2 == "" || archivoColsFinal == "")
					{
						esValido = false;
					}	
					
					break;
				case AlgebraCombTipo.SumaSoloComunes:
					
					if(archivoCols1 == "" || archivoCols2 == "" || archivoColsFinal == "")
					{
						esValido = false;
					}
					
					break;	
				case AlgebraCombTipo.RestaSegunda:
					
					if(archivoCols1 == "" || archivoCols2 == "" || archivoColsFinal == "")
					{
						esValido = false;
					}
					
					break;			
			}
            if ((noSignos2 != noSignos1) || (noSignos1 == 0) || noSignos2 == 0)
            {
                esValido = false;
                lblComb1.Text = "";
                lblComb2.Text = "";
                lblCombFinal.Text = "";
                archivoCols1 = "";
                archivoCols2 = "";
                archivoColsFinal = "";
                MessageBox.Show("Los archivos tienen distinto número de partidos", "Error", MessageBoxButtons.OK);

            }

			return esValido;
		
		}

		void BtnSelComb1Click(object sender, System.EventArgs e)
		{
            lblFantasma.Focus();
			OpenFileDialog abreFiltroDialog = new OpenFileDialog();
            abreFiltroDialog.InitialDirectory = Application.StartupPath + "/Columnas/";
			abreFiltroDialog.Filter = "Columnas(*.txt)|*.txt|Todos los archivos (*.*)|*.*" ;
					
			if(abreFiltroDialog.ShowDialog() == DialogResult.OK)
		    {		    	
		    	archivoCols1 = abreFiltroDialog.FileName;	    	
		    		    	
		    	string temp;
		    	//obten solo nombre archivo sin directorio. ej: filtro.txt
                temp = System.IO.Path.GetFileName(archivoCols1);
		    	//quitar extension. ej hola.txt -> hola
		    	temp = temp.Substring(0, temp.IndexOf('.') );		    	 		

		    	lblComb1.Text = temp;

                IArchivoColumnas archivo = new ArchivoColumnasTexto(archivoCols1);
                noSignos1 = archivo.ObtenNumSignos();
				lblFiltro1.Text="Combinación 1: " + archivo.NumColumnas.ToString() + " columnas.";
			}
            
		}
		
		void BtnSelComb2Click(object sender, System.EventArgs e)
		{
            lblFantasma.Focus();

			OpenFileDialog abreFiltroDialog = new OpenFileDialog();
            abreFiltroDialog.InitialDirectory = Application.StartupPath + "/Columnas/";
			abreFiltroDialog.Filter = "Columnas(*.txt)|*.txt|Todos los archivos (*.*)|*.*" ;
					
			if(abreFiltroDialog.ShowDialog() == DialogResult.OK)
		    {		    	
		    	archivoCols2 = abreFiltroDialog.FileName;		    	
		    		    	
		    	string temp;
		    	//obten solo nombre archivo sin directorio. ej: filtro.txt
                temp = System.IO.Path.GetFileName(archivoCols2);
		    	//quitar extension. ej hola.txt -> hola
		    	temp = temp.Substring(0, temp.IndexOf('.') );		    	 		
		    	
		    	lblComb2.Text = temp;

                IArchivoColumnas archivo = new ArchivoColumnasTexto(archivoCols2);
                noSignos2 = archivo.ObtenNumSignos();
				lblFiltro2.Text="Combinación 2: "+ archivo.NumColumnas +" columnas.";
			}

		}
		
		void BtnSelCombFinalClick(object sender, System.EventArgs e)
		{
            lblFantasma.Focus();

			//obten nombre nueva combinacion
			SaveFileDialog saveDialog = new SaveFileDialog();
			saveDialog.InitialDirectory = Application.StartupPath + "/Columnas/" ;
			saveDialog.Filter = "Columnas(*.txt)|*.txt|Todos los archivos (*.*)|*.*";
					
			if(saveDialog.ShowDialog() == DialogResult.OK)
		    {
				salidaBinaria=saveDialog.FilterIndex==2;
		    	archivoColsFinal = saveDialog.FileName;

		    	string temp;
		    	//obten solo nombre archivo sin directorio. ej: filtro.txt
                temp = System.IO.Path.GetFileName(archivoColsFinal);
		    	//quitar extension. ej hola.txt -> hola
		    	temp = temp.Substring(0, temp.IndexOf('.') );		    	 		
		    	
		    	lblCombFinal.Text = temp;		    	
		    }

		}
		
		void BtnCancelarClick(object sender, System.EventArgs e)
		{
			this.Close();
		}
		
		void BtnCalcularClick(object sender, System.EventArgs e)
		{
            lblFantasma.Focus();

			Calcula();
		}
		
		void InitializeComponent() {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AlgebraColumnasFrm));
            this.radOption2 = new System.Windows.Forms.RadioButton();
            this.radOption3 = new System.Windows.Forms.RadioButton();
            this.radOption1 = new System.Windows.Forms.RadioButton();
            this.label = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.btnCancelar = new System.Windows.Forms.Button();
            this.btnSelCombFinal = new System.Windows.Forms.Button();
            this.lblComb2 = new System.Windows.Forms.Label();
            this.lblComb1 = new System.Windows.Forms.Label();
            this.btnCalcular = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.lblFiltro2 = new System.Windows.Forms.Label();
            this.lblFiltro1 = new System.Windows.Forms.Label();
            this.lblResultado = new System.Windows.Forms.Label();
            this.lblCombFinal = new System.Windows.Forms.Label();
            this.btnSelComb2 = new System.Windows.Forms.Button();
            this.btnSelComb1 = new System.Windows.Forms.Button();
            this.groupBox = new System.Windows.Forms.GroupBox();
            this.radOption4 = new System.Windows.Forms.RadioButton();
            this.lblFantasma = new System.Windows.Forms.Label();
            this.groupBox2.SuspendLayout();
            this.groupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // radOption2
            // 
            this.radOption2.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radOption2.Location = new System.Drawing.Point(16, 56);
            this.radOption2.Name = "radOption2";
            this.radOption2.Size = new System.Drawing.Size(344, 24);
            this.radOption2.TabIndex = 2;
            this.radOption2.Text = "Suma Combinaciones: elimina columnas repetidas";
            // 
            // radOption3
            // 
            this.radOption3.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radOption3.Location = new System.Drawing.Point(16, 88);
            this.radOption3.Name = "radOption3";
            this.radOption3.Size = new System.Drawing.Size(344, 24);
            this.radOption3.TabIndex = 1;
            this.radOption3.Text = "Suma Combinaciones: selecciona columnas comunes";
            // 
            // radOption1
            // 
            this.radOption1.Checked = true;
            this.radOption1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radOption1.Location = new System.Drawing.Point(16, 24);
            this.radOption1.Name = "radOption1";
            this.radOption1.Size = new System.Drawing.Size(344, 24);
            this.radOption1.TabIndex = 0;
            this.radOption1.TabStop = true;
            this.radOption1.Text = "Elimina columnas repetidas de Combinación 1";
            // 
            // label
            // 
            this.label.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label.Location = new System.Drawing.Point(16, 16);
            this.label.Name = "label";
            this.label.Size = new System.Drawing.Size(99, 16);
            this.label.TabIndex = 0;
            this.label.Text = "Combinación 1";
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(16, 48);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(99, 16);
            this.label2.TabIndex = 1;
            this.label2.Text = "Combinación 2";
            // 
            // label3
            // 
            this.label3.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(16, 80);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(99, 16);
            this.label3.TabIndex = 2;
            this.label3.Text = "Resultado";
            // 
            // btnCancelar
            // 
            this.btnCancelar.BackColor = System.Drawing.Color.DarkSalmon;
            this.btnCancelar.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnCancelar.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancelar.Image = ((System.Drawing.Image)(resources.GetObject("btnCancelar.Image")));
            this.btnCancelar.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCancelar.Location = new System.Drawing.Point(208, 392);
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.Size = new System.Drawing.Size(100, 32);
            this.btnCancelar.TabIndex = 9;
            this.btnCancelar.Text = "Cancelar";
            this.btnCancelar.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnCancelar.UseVisualStyleBackColor = false;
            this.btnCancelar.Click += new System.EventHandler(this.BtnCancelarClick);
            // 
            // btnSelCombFinal
            // 
            this.btnSelCombFinal.BackColor = System.Drawing.Color.LightSalmon;
            this.btnSelCombFinal.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnSelCombFinal.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSelCombFinal.Image = ((System.Drawing.Image)(resources.GetObject("btnSelCombFinal.Image")));
            this.btnSelCombFinal.Location = new System.Drawing.Point(120, 76);
            this.btnSelCombFinal.Name = "btnSelCombFinal";
            this.btnSelCombFinal.Size = new System.Drawing.Size(24, 24);
            this.btnSelCombFinal.TabIndex = 5;
            this.btnSelCombFinal.TabStop = false;
            this.btnSelCombFinal.UseVisualStyleBackColor = false;
            this.btnSelCombFinal.Click += new System.EventHandler(this.BtnSelCombFinalClick);
            // 
            // lblComb2
            // 
            this.lblComb2.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblComb2.Location = new System.Drawing.Point(152, 48);
            this.lblComb2.Name = "lblComb2";
            this.lblComb2.Size = new System.Drawing.Size(180, 16);
            this.lblComb2.TabIndex = 7;
            this.lblComb2.Text = "(selecciona)";
            // 
            // lblComb1
            // 
            this.lblComb1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblComb1.Location = new System.Drawing.Point(152, 16);
            this.lblComb1.Name = "lblComb1";
            this.lblComb1.Size = new System.Drawing.Size(180, 16);
            this.lblComb1.TabIndex = 6;
            this.lblComb1.Text = "(selecciona)";
            // 
            // btnCalcular
            // 
            this.btnCalcular.BackColor = System.Drawing.Color.DarkSalmon;
            this.btnCalcular.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnCalcular.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCalcular.Image = ((System.Drawing.Image)(resources.GetObject("btnCalcular.Image")));
            this.btnCalcular.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCalcular.Location = new System.Drawing.Point(96, 392);
            this.btnCalcular.Name = "btnCalcular";
            this.btnCalcular.Size = new System.Drawing.Size(100, 32);
            this.btnCalcular.TabIndex = 10;
            this.btnCalcular.Text = "Calcular";
            this.btnCalcular.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnCalcular.UseVisualStyleBackColor = false;
            this.btnCalcular.Click += new System.EventHandler(this.BtnCalcularClick);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.lblFiltro2);
            this.groupBox2.Controls.Add(this.lblFiltro1);
            this.groupBox2.Controls.Add(this.lblResultado);
            this.groupBox2.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox2.Location = new System.Drawing.Point(16, 272);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(366, 112);
            this.groupBox2.TabIndex = 12;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Resultado";
            // 
            // lblFiltro2
            // 
            this.lblFiltro2.Location = new System.Drawing.Point(16, 48);
            this.lblFiltro2.Name = "lblFiltro2";
            this.lblFiltro2.Size = new System.Drawing.Size(304, 16);
            this.lblFiltro2.TabIndex = 2;
            // 
            // lblFiltro1
            // 
            this.lblFiltro1.Location = new System.Drawing.Point(16, 24);
            this.lblFiltro1.Name = "lblFiltro1";
            this.lblFiltro1.Size = new System.Drawing.Size(304, 16);
            this.lblFiltro1.TabIndex = 1;
            // 
            // lblResultado
            // 
            this.lblResultado.ForeColor = System.Drawing.SystemColors.ActiveCaption;
            this.lblResultado.Location = new System.Drawing.Point(16, 72);
            this.lblResultado.Name = "lblResultado";
            this.lblResultado.Size = new System.Drawing.Size(304, 32);
            this.lblResultado.TabIndex = 0;
            // 
            // lblCombFinal
            // 
            this.lblCombFinal.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCombFinal.Location = new System.Drawing.Point(152, 80);
            this.lblCombFinal.Name = "lblCombFinal";
            this.lblCombFinal.Size = new System.Drawing.Size(180, 16);
            this.lblCombFinal.TabIndex = 8;
            this.lblCombFinal.Text = "(selecciona)";
            // 
            // btnSelComb2
            // 
            this.btnSelComb2.BackColor = System.Drawing.Color.LightSalmon;
            this.btnSelComb2.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnSelComb2.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSelComb2.Image = ((System.Drawing.Image)(resources.GetObject("btnSelComb2.Image")));
            this.btnSelComb2.Location = new System.Drawing.Point(120, 44);
            this.btnSelComb2.Name = "btnSelComb2";
            this.btnSelComb2.Size = new System.Drawing.Size(24, 24);
            this.btnSelComb2.TabIndex = 4;
            this.btnSelComb2.TabStop = false;
            this.btnSelComb2.UseVisualStyleBackColor = false;
            this.btnSelComb2.Click += new System.EventHandler(this.BtnSelComb2Click);
            // 
            // btnSelComb1
            // 
            this.btnSelComb1.BackColor = System.Drawing.Color.LightSalmon;
            this.btnSelComb1.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnSelComb1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSelComb1.Image = ((System.Drawing.Image)(resources.GetObject("btnSelComb1.Image")));
            this.btnSelComb1.Location = new System.Drawing.Point(120, 12);
            this.btnSelComb1.Name = "btnSelComb1";
            this.btnSelComb1.Size = new System.Drawing.Size(24, 24);
            this.btnSelComb1.TabIndex = 3;
            this.btnSelComb1.TabStop = false;
            this.btnSelComb1.UseVisualStyleBackColor = false;
            this.btnSelComb1.Click += new System.EventHandler(this.BtnSelComb1Click);
            // 
            // groupBox
            // 
            this.groupBox.Controls.Add(this.radOption4);
            this.groupBox.Controls.Add(this.radOption2);
            this.groupBox.Controls.Add(this.radOption3);
            this.groupBox.Controls.Add(this.radOption1);
            this.groupBox.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox.Location = new System.Drawing.Point(16, 112);
            this.groupBox.Name = "groupBox";
            this.groupBox.Size = new System.Drawing.Size(366, 152);
            this.groupBox.TabIndex = 11;
            this.groupBox.TabStop = false;
            this.groupBox.Text = "Operaciones";
            // 
            // radOption4
            // 
            this.radOption4.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radOption4.Location = new System.Drawing.Point(16, 120);
            this.radOption4.Name = "radOption4";
            this.radOption4.Size = new System.Drawing.Size(344, 24);
            this.radOption4.TabIndex = 3;
            this.radOption4.Text = "Resta combinaciones: (1)-(2)";
            // 
            // lblFantasma
            // 
            this.lblFantasma.AutoSize = true;
            this.lblFantasma.Location = new System.Drawing.Point(357, 413);
            this.lblFantasma.Name = "lblFantasma";
            this.lblFantasma.Size = new System.Drawing.Size(0, 13);
            this.lblFantasma.TabIndex = 13;
            // 
            // AlgebraColumnasFrm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.Color.Bisque;
            this.ClientSize = new System.Drawing.Size(404, 438);
            this.ControlBox = false;
            this.Controls.Add(this.lblFantasma);
            this.Controls.Add(this.groupBox);
            this.Controls.Add(this.btnCalcular);
            this.Controls.Add(this.btnCancelar);
            this.Controls.Add(this.lblCombFinal);
            this.Controls.Add(this.lblComb2);
            this.Controls.Add(this.lblComb1);
            this.Controls.Add(this.btnSelCombFinal);
            this.Controls.Add(this.btnSelComb2);
            this.Controls.Add(this.btnSelComb1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label);
            this.Controls.Add(this.groupBox2);
            this.Name = "AlgebraColumnasFrm";
            this.Text = "Álgebra";
            this.groupBox2.ResumeLayout(false);
            this.groupBox.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		
	}
}
