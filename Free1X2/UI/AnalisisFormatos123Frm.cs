// Free1X2 : Programa de quinielas "libre"
// Copyright (C) 2007 Morrison - morrison.ne@gmail.com
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
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using System.IO;

using Free1X2.MotorCalculo;
using Free1X2.EntradaSalida;

using Free1X2.UI.Modern.Theming;
namespace Free1X2.UI
{
	/// <summary>
	/// Summary description for AnalisisFormatos123sFrm.
	/// </summary>
	public class AnalisisFormatos123Frm : System.Windows.Forms.Form
	{
		private Controls.valors valors1;
		private ContainerControl cctrl;
        protected List<Formato123> arrayFormatos;
		private Button btnAnalisis;
		private Label lugar;
		private Button bMenos;
		private Button bMas;
		private Button bLeeGrupo;
        protected List<string> arrayColumnas = new List<string>();
		private Label lblNumCol;
		protected int noColumna;
		private Label lblCGanadora;
        private Button button1;
        private Label label1;
        private Label lblFantasma;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public AnalisisFormatos123Frm()
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

        public List<Formato123> ArrayFormatos
		{
			get {return arrayFormatos;}
			set {arrayFormatos = value;}
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

		protected void ObtenerFormatos(string columna1x2)
		{
			cctrl.Controls.Clear();

			string columnaFormato = TraducirColumna(columna1x2,TransformarValoracion(valors1.RetVals()));
			long columnaFormatoBin = ConvStrToLong(columnaFormato);
			int posicionXInicial = 22;
			int posicionYInicial = 1;

			for (int i = 0; i < ArrayFormatos.Count; i++)
			{
				Formato123 formato = ArrayFormatos[i];
				long formato123 = ConvStrToLong(formato.Formato);
				int apariciones = DeterminaApariciones(columnaFormatoBin, formato123);
				MostrarFormato(formato,apariciones,posicionXInicial, posicionYInicial);
				posicionYInicial += 15;
			}
		}
        protected void ObtenerTodosFormatos(string columna1x2)
        {
            cctrl.Controls.Clear();
            List<Formato123> todosFormatos = new List<Formato123>();
            string columnaFormato = TraducirColumna(columna1x2, TransformarValoracion(valors1.RetVals()));
            long columnaFormatoBin = ConvStrToLong(columnaFormato);
            int posicionXInicial = 22;
            int posicionYInicial = 1;

            for (int longitud = 1; longitud <= columna1x2.Length; longitud++)
            {
                for (int posicion = 0; posicion < columna1x2.Length; posicion ++)
                {

                    if ((posicion + longitud) <= columna1x2.Length)
                    {
                        Formato123 formato = new Formato123();
                        formato.Formato = columnaFormato.Substring(posicion, longitud);
                        todosFormatos.Add(formato);
                    }
                    else
                    {
                        break;
                    }
                }
            }

            //tenemos lista la lista de todos los formatos. Ahora vamos a contarlos
            for (int i = 0; i < todosFormatos.Count; i++)
            {
                Formato123 formato = todosFormatos[i];
                formato.AciertosMax = 1;
                for (int j = i + 1; j < todosFormatos.Count; j++)
                {
                    Formato123 formato2 = todosFormatos[j];
                    if (formato2.Formato == formato.Formato)
                    {
                        formato.AciertosMax++;
                        todosFormatos.RemoveAt(j);
                        j--;
                    }

                }
            }

            //Ahora mostrar los formatos
            for (int i = 0; i < todosFormatos.Count; i++)
            {
                MostrarFormato(todosFormatos[i], todosFormatos[i].AciertosMax, posicionXInicial, posicionYInicial);
                posicionYInicial += 15;
            }
        }

        protected string TraducirColumna(string columna, string[,] val)
        {
            string columnaAFormato = "";
            for (int i = 0; i < columna.Length; i++)
            {
                if (columna[i] == '1')
                {
                    columnaAFormato += val[i, 0];
                }
                else if (columna[i] == 'X')
                {
                    columnaAFormato += val[i, 1];
                }
                else
                {
                    columnaAFormato += val[i, 2];
                }
            }

            return columnaAFormato;
        }
        protected string[,] TransformarValoracion(double[,] valoracion)
        {
            string[,] valoresTransformados = new string[valoracion.GetLength(0), 3];
            for (int i = 0; i < valoracion.GetLength(0); i++)
            {
                double[] valor = { valoracion[i, 0], valoracion[i, 1], valoracion[i, 2] };
                if ((valor[0] >= valor[1]) && (valor[0] >= valor[2]))
                {
                    if (valor[1] >= valor[2])
                    {
                        valoresTransformados[i, 0] = "1";
                        valoresTransformados[i, 1] = "2";
                        valoresTransformados[i, 2] = "3";
                    }
                    else if (valor[2] > valor[1])
                    {
                        valoresTransformados[i, 0] = "1";
                        valoresTransformados[i, 1] = "3";
                        valoresTransformados[i, 2] = "2";
                    }
                }
                else if ((valor[1] > valor[0]) && (valor[1] >= valor[2]))
                {
                    if (valor[0] >= valor[2])
                    {
                        valoresTransformados[i, 0] = "2";
                        valoresTransformados[i, 1] = "1";
                        valoresTransformados[i, 2] = "3";
                    }
                    else
                    {
                        valoresTransformados[i, 0] = "3";
                        valoresTransformados[i, 1] = "1";
                        valoresTransformados[i, 2] = "2";
                    }
                }
                else if ((valor[2] > valor[0]) && (valor[2] > valor[1]))
                {
                    if (valor[0] >= valor[1])
                    {
                        valoresTransformados[i, 0] = "2";
                        valoresTransformados[i, 1] = "3";
                        valoresTransformados[i, 2] = "1";

                    }
                    else
                    {
                        valoresTransformados[i, 0] = "3";
                        valoresTransformados[i, 1] = "2";
                        valoresTransformados[i, 2] = "1";
                    }
                }
            } 
            return valoresTransformados;
        }
        private long ConvStrToLong(string s)
		{
			string signos = "321";
			long res=0;
			for(int i=s.Length-1; i>-1;i--)
			{
				res =(res <<=3) ^ (1<<signos.IndexOf (s.Substring (i,1)));
			}
			return res;
		}
		protected int DeterminaApariciones(long columnaFormato, long formato)
		{
			int aciertos = 0;
				
			while (columnaFormato != 0 )
			{
				if (((columnaFormato) & formato) == formato)
				{
					aciertos++;
				}
				columnaFormato >>= 3;
			} 
			return aciertos;

		}
		protected void MostrarFormato(Formato123 formato, int apariciones, int posicionX, int posicionY)
		{
			Free1X2.UI.Controls.CtrlFormatos123Analisis ctrl = new Free1X2.UI.Controls.CtrlFormatos123Analisis();
			ctrl.Formato = formato;
			ctrl.Apariciones = apariciones;
			ctrl.Location = new Point(posicionX,posicionY);
			this.cctrl.Controls.Add(ctrl);
			this.cctrl.AutoScroll = true;
		}
		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AnalisisFormatos123Frm));
            this.cctrl = new System.Windows.Forms.ContainerControl();
            this.btnAnalisis = new System.Windows.Forms.Button();
            this.lugar = new System.Windows.Forms.Label();
            this.lblNumCol = new System.Windows.Forms.Label();
            this.bMenos = new System.Windows.Forms.Button();
            this.bMas = new System.Windows.Forms.Button();
            this.bLeeGrupo = new System.Windows.Forms.Button();
            this.lblCGanadora = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.valors1 = new Free1X2.UI.Controls.valors();
            this.lblFantasma = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // cctrl
            // 
            this.cctrl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cctrl.BackColor = System.Drawing.Color.Khaki;
            this.cctrl.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cctrl.Location = new System.Drawing.Point(182, 37);
            this.cctrl.Name = "cctrl";
            this.cctrl.Size = new System.Drawing.Size(198, 275);
            this.cctrl.TabIndex = 1;
            this.cctrl.Text = "containerControl1";
            // 
            // btnAnalisis
            // 
            this.btnAnalisis.BackColor = System.Drawing.Color.DarkSalmon;
            this.btnAnalisis.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnAnalisis.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAnalisis.Image = ((System.Drawing.Image)(resources.GetObject("btnAnalisis.Image")));
            this.btnAnalisis.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnAnalisis.Location = new System.Drawing.Point(201, 459);
            this.btnAnalisis.Name = "btnAnalisis";
            this.btnAnalisis.Size = new System.Drawing.Size(163, 32);
            this.btnAnalisis.TabIndex = 168;
            this.btnAnalisis.Text = "Analizar";
            this.btnAnalisis.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnAnalisis.UseVisualStyleBackColor = false;
            this.btnAnalisis.Click += new System.EventHandler(this.btnAnalisis_Click);
            // 
            // lugar
            // 
            this.lugar.BackColor = System.Drawing.SystemColors.Info;
            this.lugar.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lugar.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lugar.Location = new System.Drawing.Point(201, 315);
            this.lugar.Name = "lugar";
            this.lugar.Size = new System.Drawing.Size(163, 24);
            this.lugar.TabIndex = 169;
            this.lugar.Text = "Ganadoras";
            this.lugar.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblNumCol
            // 
            this.lblNumCol.BackColor = System.Drawing.SystemColors.Info;
            this.lblNumCol.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblNumCol.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNumCol.Location = new System.Drawing.Point(201, 418);
            this.lblNumCol.Name = "lblNumCol";
            this.lblNumCol.Size = new System.Drawing.Size(146, 33);
            this.lblNumCol.TabIndex = 172;
            this.lblNumCol.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // bMenos
            // 
            this.bMenos.BackColor = System.Drawing.Color.DarkSalmon;
            this.bMenos.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.bMenos.Font = new System.Drawing.Font("Verdana", 6F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bMenos.Location = new System.Drawing.Point(348, 435);
            this.bMenos.Name = "bMenos";
            this.bMenos.Size = new System.Drawing.Size(16, 16);
            this.bMenos.TabIndex = 171;
            this.bMenos.Text = "-";
            this.bMenos.UseVisualStyleBackColor = false;
            this.bMenos.Click += new System.EventHandler(this.bMenos_Click);
            // 
            // bMas
            // 
            this.bMas.BackColor = System.Drawing.Color.DarkSalmon;
            this.bMas.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.bMas.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.bMas.Font = new System.Drawing.Font("Verdana", 6F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bMas.Location = new System.Drawing.Point(348, 418);
            this.bMas.Name = "bMas";
            this.bMas.Size = new System.Drawing.Size(16, 16);
            this.bMas.TabIndex = 170;
            this.bMas.TabStop = false;
            this.bMas.Text = "+";
            this.bMas.UseVisualStyleBackColor = false;
            this.bMas.Click += new System.EventHandler(this.bMas_Click);
            // 
            // bLeeGrupo
            // 
            this.bLeeGrupo.BackColor = System.Drawing.Color.DarkSalmon;
            this.bLeeGrupo.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.bLeeGrupo.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bLeeGrupo.Image = ((System.Drawing.Image)(resources.GetObject("bLeeGrupo.Image")));
            this.bLeeGrupo.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.bLeeGrupo.Location = new System.Drawing.Point(201, 379);
            this.bLeeGrupo.Name = "bLeeGrupo";
            this.bLeeGrupo.Size = new System.Drawing.Size(163, 32);
            this.bLeeGrupo.TabIndex = 173;
            this.bLeeGrupo.Text = "Archivo de Columnas";
            this.bLeeGrupo.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.bLeeGrupo.UseVisualStyleBackColor = false;
            this.bLeeGrupo.Click += new System.EventHandler(this.bLeeGrupo_Click);
            // 
            // lblCGanadora
            // 
            this.lblCGanadora.BackColor = System.Drawing.SystemColors.Info;
            this.lblCGanadora.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblCGanadora.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCGanadora.Location = new System.Drawing.Point(201, 347);
            this.lblCGanadora.Name = "lblCGanadora";
            this.lblCGanadora.Size = new System.Drawing.Size(163, 24);
            this.lblCGanadora.TabIndex = 175;
            this.lblCGanadora.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.Color.DarkSalmon;
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.button1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.Image = ((System.Drawing.Image)(resources.GetObject("button1.Image")));
            this.button1.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button1.Location = new System.Drawing.Point(201, 497);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(163, 32);
            this.button1.TabIndex = 176;
            this.button1.Text = "Mostrar Todos";
            this.button1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.Color.LightSalmon;
            this.label1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(182, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(198, 21);
            this.label1.TabIndex = 177;
            this.label1.Text = "Informe";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // valors1
            // 
            this.valors1.BackColor = System.Drawing.Color.Bisque;
            this.valors1.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.valors1.Location = new System.Drawing.Point(8, 8);
            this.valors1.Name = "valors1";
            this.valors1.Size = new System.Drawing.Size(168, 520);
            this.valors1.TabIndex = 0;
            // 
            // lblFantasma
            // 
            this.lblFantasma.AutoSize = true;
            this.lblFantasma.Location = new System.Drawing.Point(377, 437);
            this.lblFantasma.Name = "lblFantasma";
            this.lblFantasma.Size = new System.Drawing.Size(0, 13);
            this.lblFantasma.TabIndex = 179;
            // 
            // AnalisisFormatos123Frm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.Color.Bisque;
            this.ClientSize = new System.Drawing.Size(402, 540);
            this.Controls.Add(this.lblFantasma);
            this.Controls.Add(this.valors1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.lblCGanadora);
            this.Controls.Add(this.bLeeGrupo);
            this.Controls.Add(this.lblNumCol);
            this.Controls.Add(this.bMenos);
            this.Controls.Add(this.bMas);
            this.Controls.Add(this.lugar);
            this.Controls.Add(this.btnAnalisis);
            this.Controls.Add(this.cctrl);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AnalisisFormatos123Frm";
            this.Text = "Analizador Formatos123";
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		private void btnAnalisis_Click(object sender, System.EventArgs e)
		{
            Analizar();
		}
        private void Analizar()
        {
            if (this.arrayColumnas.Count > 0)
            {
                ObtenerFormatos(this.arrayColumnas[this.noColumna]);
            }
        }
		protected void EntradaFichero() 
		{
			string columna;
			OpenFileDialog abreArchivo = new OpenFileDialog();
            abreArchivo.InitialDirectory = Application.StartupPath + "/";
			abreArchivo.Filter = "Columnas(*.txt)|*.txt|Columnas(*.cols)|*.cols|Todos los archivos(*.*)|*.*" ;
			if(abreArchivo.ShowDialog() == DialogResult.OK) 
			{
			    arrayColumnas.Clear();
                IArchivoColumnas ac = new ArchivoColumnasTexto(abreArchivo.FileName);
				while(ac.SiguienteColumna())
				{
					columna = ac.LeeColumnaSinComas();
					if(columna.Length > 16)
					{
						MessageBox.Show("Error leyendo columnas");
						arrayColumnas.Clear();
						break;
					}
				    arrayColumnas.Add(columna.Substring(0,columna.Length).ToUpper());
				}
				ac.Cerrar();
				btnAnalisis.Enabled = true;
			}
		}

		private void bLeeGrupo_Click(object sender, System.EventArgs e)
		{
			EntradaFichero();
			MostrarContador();
		}
		protected void MostrarColumnaAnalizada()
		{
			if(this.noColumna < this.arrayColumnas.Count)
			{
				string cGanadora = arrayColumnas[this.noColumna];
				this.lblCGanadora.Text = cGanadora;
				//MostrarContador();
			}
		}
		protected void MostrarContador()
		{
			int columnaActual = 0;
			if(this.arrayColumnas.Count > 0)
			{
				columnaActual = this.noColumna + 1;
			}
			
			lblNumCol.Text = columnaActual.ToString() + "/" + this.arrayColumnas.Count.ToString();
			MostrarColumnaAnalizada();
		}
		protected void IncrementarContador()
		{
			
			if(this.noColumna < this.arrayColumnas.Count)
			{
				this.noColumna++;
				MostrarContador();
			}
		}
		protected void DecrementarContador()
		{
			
			if(this.noColumna > 0)
			{	
				this.noColumna--;
				MostrarContador();
			}
		}
		private void bMas_Click(object sender, System.EventArgs e)
		{
			IncrementarContador();
			MostrarContador();
            this.lblFantasma.Focus();
		}

		private void bMenos_Click(object sender, System.EventArgs e)
		{
			DecrementarContador();
			MostrarContador();
            this.lblFantasma.Focus();
		}

        private void button1_Click(object sender, EventArgs e)
        {
            if (this.arrayColumnas.Count > 0)
            {
                ObtenerTodosFormatos(this.arrayColumnas[this.noColumna]);
            }
        }

	}

}
