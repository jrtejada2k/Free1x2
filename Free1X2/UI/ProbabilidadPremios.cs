using System;
using System.Windows.Forms;
using System.Collections.Generic;
using Free1X2.Utils;
using Free1X2.Analisis;

namespace Free1X2.UI
{
	public class ProbabilidadPremios : System.Windows.Forms.Form
	{
        private System.Windows.Forms.Label label;
		private System.Windows.Forms.Button btnSelectHija;
		private System.Windows.Forms.Button btnSelectMadre;
        private System.Windows.Forms.Button button;
		private System.Windows.Forms.Label lblCombHija;
        private System.Windows.Forms.Label lblCombMadre;
        private System.Windows.Forms.Label label2;
		
		private string archivoColsBase = "";
        private TextBox txtRango;
        private Label label1;
		private	string archivoCols = "";
        private int minimoPremio = 0;
        private int maximoPremio = 0;
		public ProbabilidadPremios()
		{
			InitializeComponent();
			//no activar hasta que las Dos combinaciones esten selecionadas
			button.Enabled = false;
            FormulariosHelper fH = new FormulariosHelper();
            fH.Traducir(this);
		}
		
		protected void ActivarBotonCalculo()
		{
			if(archivoColsBase != "" && archivoCols != "")
			{			
				button.Enabled = true;
			}
		
		}
		
		
		
		void ButtonClick(object sender, System.EventArgs e)
		{
            //Obtener Rango premios
            if (txtRango.Text != "")
            {
                try
                {
                    List<int> premioMinimo = UtilidadesEntradasValores.ObtenerListaFromTxtAciertos(txtRango.Text);
                    premioMinimo.Sort();
                    minimoPremio = premioMinimo[0];
                    maximoPremio = premioMinimo[premioMinimo.Count - 1];
                }
                catch
                {
                    minimoPremio = 10;
                    maximoPremio = 14;
                }
            }
            else
            {
                minimoPremio = 10;
                maximoPremio = 14;
            }
            button.Enabled = false;
						
			Analizador analizador = new Analizador(minimoPremio);
			
			analizador.ComparaCombinaciones( archivoColsBase, archivoCols);
            int x = 64;
            int y = 88;

            for (int i = minimoPremio; i <= maximoPremio; i++)
            {
                Label lbl = new Label();
                lbl.Size = new System.Drawing.Size(188, 16);
                lbl.Text = "Premio de " + i.ToString() + ": " + analizador.ObtenProbabilidadPremios(i).ToString("#,##0.00;0.00") + " %";
                lbl.Location = new System.Drawing.Point(x, y);
                lbl.Name = "lbl_" + i.ToString();
                this.Controls.Add(lbl);
                y += 17;
                lbl = new Label();
            }
			button.Enabled = true;
		}
		
		void btnSelectMadreClick(object sender, System.EventArgs e)
		{
			OpenFileDialog abreFiltroDialog = new OpenFileDialog();
			abreFiltroDialog.InitialDirectory = "Columnas\\" ;
			abreFiltroDialog.Filter = "Columnas(*.txt)|*.txt|Todos los archivos (*.*)|*.*" ;
					
			if(abreFiltroDialog.ShowDialog() == DialogResult.OK)
		    {		    	
		    	archivoColsBase = abreFiltroDialog.FileName;		    	

		    	string temp;
		    	//obten solo nombre archivo sin directorio. ej: filtro.txt
		    	//temp = archivoColsBase.Substring( archivoColsBase.LastIndexOf("\\") + 1 );
                temp = System.IO.Path.GetFileNameWithoutExtension(archivoColsBase);
		    	//quitar extension. ej hola.txt -> hola
		    	//temp = temp.Substring(0, temp.IndexOf('.') );		    	 		
		    	
		    	lblCombMadre.Text = temp;
		    }
		    
		    ActivarBotonCalculo();
		}
		
		void btnSelectHijaClick(object sender, System.EventArgs e)
		{
			OpenFileDialog abreFiltroDialog = new OpenFileDialog();
			abreFiltroDialog.InitialDirectory = "Columnas\\" ;
			abreFiltroDialog.Filter = "Columnas(*.txt)|*.txt|Todos los archivos (*.*)|*.*" ;
					
			if(abreFiltroDialog.ShowDialog() == DialogResult.OK)
		    {		    	
		    	archivoCols = abreFiltroDialog.FileName;		    	
		    		    	
		    	string temp;
		    	//obten solo nombre archivo sin directorio. ej: filtro.txt
                temp = System.IO.Path.GetFileName(archivoCols);
		    	//quitar extension. ej hola.txt -> hola
                temp = System.IO.Path.GetFileNameWithoutExtension(temp);
		    	lblCombHija.Text = temp;
		    }
		    
		    ActivarBotonCalculo();
		}
		
		void InitializeComponent() {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProbabilidadPremios));
            this.label2 = new System.Windows.Forms.Label();
            this.lblCombMadre = new System.Windows.Forms.Label();
            this.lblCombHija = new System.Windows.Forms.Label();
            this.button = new System.Windows.Forms.Button();
            this.btnSelectMadre = new System.Windows.Forms.Button();
            this.btnSelectHija = new System.Windows.Forms.Button();
            this.label = new System.Windows.Forms.Label();
            this.txtRango = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.Maroon;
            this.label2.Location = new System.Drawing.Point(16, 44);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(130, 24);
            this.label2.TabIndex = 2;
            this.label2.Text = "Columnas \"hijas\"";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblCombMadre
            // 
            this.lblCombMadre.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCombMadre.Location = new System.Drawing.Point(184, 12);
            this.lblCombMadre.Name = "lblCombMadre";
            this.lblCombMadre.Size = new System.Drawing.Size(103, 24);
            this.lblCombMadre.TabIndex = 15;
            this.lblCombMadre.Text = "(seleccionar)";
            this.lblCombMadre.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblCombHija
            // 
            this.lblCombHija.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCombHija.Location = new System.Drawing.Point(184, 44);
            this.lblCombHija.Name = "lblCombHija";
            this.lblCombHija.Size = new System.Drawing.Size(103, 24);
            this.lblCombHija.TabIndex = 16;
            this.lblCombHija.Text = "(seleccionar)";
            this.lblCombHija.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // button
            // 
            this.button.BackColor = System.Drawing.Color.DarkSalmon;
            this.button.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.button.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button.Image = ((System.Drawing.Image)(resources.GetObject("button.Image")));
            this.button.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button.Location = new System.Drawing.Point(272, 224);
            this.button.Name = "button";
            this.button.Size = new System.Drawing.Size(128, 32);
            this.button.TabIndex = 0;
            this.button.Text = "Calcular";
            this.button.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.button.UseVisualStyleBackColor = false;
            this.button.Click += new System.EventHandler(this.ButtonClick);
            // 
            // btnSelectMadre
            // 
            this.btnSelectMadre.BackColor = System.Drawing.Color.LightSalmon;
            this.btnSelectMadre.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnSelectMadre.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSelectMadre.Image = ((System.Drawing.Image)(resources.GetObject("btnSelectMadre.Image")));
            this.btnSelectMadre.Location = new System.Drawing.Point(152, 12);
            this.btnSelectMadre.Name = "btnSelectMadre";
            this.btnSelectMadre.Size = new System.Drawing.Size(24, 24);
            this.btnSelectMadre.TabIndex = 13;
            this.btnSelectMadre.UseVisualStyleBackColor = false;
            this.btnSelectMadre.Click += new System.EventHandler(this.btnSelectMadreClick);
            // 
            // btnSelectHija
            // 
            this.btnSelectHija.BackColor = System.Drawing.Color.LightSalmon;
            this.btnSelectHija.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnSelectHija.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSelectHija.Image = ((System.Drawing.Image)(resources.GetObject("btnSelectHija.Image")));
            this.btnSelectHija.Location = new System.Drawing.Point(152, 44);
            this.btnSelectHija.Name = "btnSelectHija";
            this.btnSelectHija.Size = new System.Drawing.Size(24, 24);
            this.btnSelectHija.TabIndex = 14;
            this.btnSelectHija.UseVisualStyleBackColor = false;
            this.btnSelectHija.Click += new System.EventHandler(this.btnSelectHijaClick);
            // 
            // label
            // 
            this.label.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label.ForeColor = System.Drawing.Color.Maroon;
            this.label.Location = new System.Drawing.Point(5, 12);
            this.label.Name = "label";
            this.label.Size = new System.Drawing.Size(141, 24);
            this.label.TabIndex = 1;
            this.label.Text = "Columnas \"madre\"";
            this.label.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtRango
            // 
            this.txtRango.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtRango.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtRango.Location = new System.Drawing.Point(300, 44);
            this.txtRango.Name = "txtRango";
            this.txtRango.Size = new System.Drawing.Size(100, 21);
            this.txtRango.TabIndex = 17;
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(297, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(103, 16);
            this.label1.TabIndex = 18;
            this.label1.Text = "Rango Premios";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ProbabilidadPremios
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.Color.Bisque;
            this.ClientSize = new System.Drawing.Size(408, 266);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtRango);
            this.Controls.Add(this.lblCombHija);
            this.Controls.Add(this.lblCombMadre);
            this.Controls.Add(this.btnSelectHija);
            this.Controls.Add(this.btnSelectMadre);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label);
            this.Controls.Add(this.button);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ProbabilidadPremios";
            this.Text = "Probabilidades de Premios";
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		
	}
}
