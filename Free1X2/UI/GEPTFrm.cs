// Free1X2 : Programa de quinielas "libre"
// Copyright (C) 2004 xfsf
// Según una idea de Jose Carlos de Nova, ligeramente modificada por xfsf
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

namespace Free1X2.UI
{
    public class GEPTFrm : Form
    {
        private Button bCalcular;
		private Label label1;
        private Label label7;
        private System.ComponentModel.IContainer components;
        private TextBox[] primeraColumna = new TextBox[VariablesGlobales.NumeroPartidos];
        private TextBox[] segundaColumna = new TextBox[VariablesGlobales.NumeroPartidos];
        private Label[] columnaResultados = new Label[VariablesGlobales.NumeroPartidos];

		public GEPTFrm()
		{
			InitializeComponent();
            FormulariosHelper fH = new FormulariosHelper();
            fH.Traducir(this);
            AñadirControles(40, 40, "p_");
            AñadirControles(75, 40, "s_");
            AñadirControles(175, 40, "r_");

		}
        private void AñadirControles(int x, int y, string plantilla)
        {
            int p;
            switch (plantilla)
            {
                case "p_":
                    for (int i = 0; i < VariablesGlobales.NumeroPartidos; i++)
                    {
                        p = i + 1;
                        TextBox txt = new TextBox();
                        Label lbl = new Label();
                        lbl.Font = new System.Drawing.Font("Verdana", 8);
                        lbl.AutoSize = false;
                        lbl.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
                        lbl.Size = new System.Drawing.Size(25, 20);
                        lbl.BackColor = System.Drawing.Color.Bisque;
                        lbl.Text = p.ToString();
                        lbl.Location = new System.Drawing.Point(10, y);
                        lbl.BorderStyle = BorderStyle.None;
                        lbl.Name = "numPartido" + p.ToString("00");

                        txt.Font = new System.Drawing.Font("Verdana", 8, System.Drawing.FontStyle.Bold);
                        txt.Size = new System.Drawing.Size(24, 24);
                        txt.TextAlign = HorizontalAlignment.Center;
                        txt.BackColor = System.Drawing.Color.White;
                        txt.Location = new System.Drawing.Point(x, y);
                        txt.BorderStyle = BorderStyle.FixedSingle;
                        txt.Name = plantilla + p.ToString("00");
                        txt.TextChanged += Resultado_TextChanged;
                        Controls.Add(txt);
                        Controls.Add(lbl);
                        y += txt.Size.Height + 1;
                        if (p % 4 == 0)
                        {
                            y += 5;
                        }
                        primeraColumna[i] = txt;
                    }
                    break;
                case "s_":
                    for (int i = 0; i < VariablesGlobales.NumeroPartidos; i++)
                    {
                        p = i + 1;
                        TextBox txt = new TextBox();
                        txt.Font = new System.Drawing.Font("Verdana", 8, System.Drawing.FontStyle.Bold);
                        txt.TextAlign = HorizontalAlignment.Center;
                        txt.Size = new System.Drawing.Size(24, 24);
                        txt.BackColor = System.Drawing.Color.White;
                        txt.Location = new System.Drawing.Point(x, y);
                        txt.BorderStyle = BorderStyle.FixedSingle;
                        txt.Name = plantilla + p.ToString("00");
                        txt.TextChanged += Resultado_TextChanged;
                        Controls.Add(txt);

                        y += txt.Size.Height + 1;
                        if (p % 4 == 0)
                        {
                            y += 5;
                        }
                        segundaColumna[i] = txt;
                    }
                    break;
                case "r_":
                    for (int i = 0; i < VariablesGlobales.NumeroPartidos; i++)
                    {
                        p = i + 1;
                        Label lbl = new Label();
                        lbl.Font = new System.Drawing.Font("Verdana", 8, System.Drawing.FontStyle.Bold);
                        lbl.AutoSize = false;
                        lbl.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
                        lbl.Size = new System.Drawing.Size(50, 20);
                        lbl.BackColor = System.Drawing.Color.NavajoWhite;
                        lbl.Location = new System.Drawing.Point(x, y);
                        lbl.BorderStyle = BorderStyle.FixedSingle;
                        lbl.Name = plantilla + p.ToString("00");
                        Controls.Add(lbl);

                        y += lbl.Size.Height + 1;
                        if (p % 4 == 0)
                        {
                            y += 5;
                        }
                        columnaResultados[i] = lbl;
                    }
                    break;
            }
        }
        private void Calcular()
        {
			Verificar();
			Pronosticar();
		}
		private void Verificar() {
			string ch, aux = "GEP"; int idx;

            for (int i = 0; i < primeraColumna.Length; i++)
            {
                ch = primeraColumna[i].Text.ToUpper();
                idx = aux.IndexOf(ch);
                if (ch == "" || idx < 0)
                {
                    idx = 3;
                }
                primeraColumna[i].BackColor = Fondo(idx);
            }
            for (int i = 0; i < segundaColumna.Length; i++)
            {
                ch = segundaColumna[i].Text.ToUpper();
                idx = aux.IndexOf(ch);
                if (ch == "" || idx < 0)
                {
                    idx = 3;
                }
                segundaColumna[i].BackColor = Fondo(idx);
            }
			return;
		}
		private System.Drawing.Color Fondo (int idx)
		{
		    if (idx==0) return System.Drawing.Color.GreenYellow;
		    if (idx==1) return System.Drawing.Color.Yellow;
		    if (idx==2) return System.Drawing.Color.Pink;
		    return System.Drawing.Color.White;
		}

        private void Pronosticar() {

            for (int i = 0; i < columnaResultados.Length; i++)
            {
                columnaResultados[i].Text = Traductor(primeraColumna[i].Text + segundaColumna[i].Text);
            }
		}
		private string Traductor(string ch) {
			string rsl;
			switch (ch.ToUpper()) {
				case "GG": rsl="12"; break;
				case "GE": rsl="1X"; break;
				case "GP": rsl="1"; break;
				case "EG": rsl="X2"; break;
				case "EE": rsl="X"; break;
				case "EP": rsl="1X"; break;
				case "PG": rsl="2"; break;
				case "PE": rsl="X2"; break;
				case "PP": rsl="12"; break;
				default: rsl=" "; break;
			}
			return rsl;
		}	
		
		
		#region Windows Forms Designer generated code
		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GEPTFrm));
            this.label7 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.bCalcular = new System.Windows.Forms.Button();
		    this.SuspendLayout();
            // 
            // label7
            // 
            this.label7.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.ForeColor = System.Drawing.Color.Maroon;
            this.label7.Location = new System.Drawing.Point(150, 16);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(100, 24);
            this.label7.TabIndex = 29;
            this.label7.Text = "Pronóstico";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.Maroon;
            this.label1.Location = new System.Drawing.Point(10, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(100, 24);
            this.label1.TabIndex = 20;
            this.label1.Text = "Última vez";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // bCalcular
            // 
            this.bCalcular.BackColor = System.Drawing.Color.DarkSalmon;
            this.bCalcular.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.bCalcular.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bCalcular.Image = ((System.Drawing.Image)(resources.GetObject("bCalcular.Image")));
            this.bCalcular.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.bCalcular.Location = new System.Drawing.Point(85, 419);
            this.bCalcular.Name = "bCalcular";
            this.bCalcular.Size = new System.Drawing.Size(128, 32);
            this.bCalcular.TabIndex = 28;
            this.bCalcular.Text = "Calcular";
            this.bCalcular.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.bCalcular.UseVisualStyleBackColor = false;
            this.bCalcular.Click += new System.EventHandler(this.BCalcularClick);
            // 
            // GEPTFrm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.Color.Bisque;
            this.ClientSize = new System.Drawing.Size(299, 463);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.bCalcular);
            this.Controls.Add(this.label7);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "GEPTFrm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Columna estadística de ABDON (GEPT)";
            this.ResumeLayout(false);

		}
		#endregion
		void BCalcularClick(object sender, EventArgs e) 
        { 
            Calcular(); 
        }

		private void Resultado_TextChanged(object sender, EventArgs e)
		{
			//obtenemos una referecia al texbox del que se acaba de cambiar el texto.
			TextBox txtBox = (TextBox)sender;

			string valor = ((txtBox.Text).Trim()).ToUpper();

			if(valor == "G")
			{
				txtBox.BackColor = System.Drawing.Color.GreenYellow;			
			}
			else if(valor == "E")
			{
				txtBox.BackColor = System.Drawing.Color.Yellow;
			}
			else if(valor == "P")
			{
				txtBox.BackColor = System.Drawing.Color.Pink;
			}
			else if(valor == "")
			{	
				txtBox.BackColor = System.Drawing.Color.White;
			}	
			else if(valor != "")
			{	
				txtBox.BackColor = System.Drawing.Color.White;
				valor = "";
				string msg = "Las letras permitidas son: G,E,P";
				MessageBox.Show(msg, "Datos incorrectos!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
			}			

			//esto convierte las minusculas en mayusculas.
			txtBox.Text = valor;
		}

		
	}
}
