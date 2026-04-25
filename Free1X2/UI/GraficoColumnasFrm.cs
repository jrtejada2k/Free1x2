// created on 28/02/2004 at 14:43
// Free1X2 : Programa de quinielas "libre"
// Copyright (C) 2004 Toni moreno
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
using System.Windows.Forms;
using Free1X2.EntradaSalida;

using Free1X2.UI.Modern.Theming;
namespace Free1X2.UI
{
	/// <summary>
	/// Descripción breve de GraficoColumnasFrm.
	/// </summary>
	public class GraficoColumnasFrm : Form
	{
		protected bool inicio;
		protected int minimo;
		protected int maximo;
		private PictureBox pictureBox1;
		private Button btnAbrir;
		private Button btnSalir;
        private Button btnGuia;
	    private bool para;
        private Label label1;
		/// <summary>
		/// Variable del diseñador requerida.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public GraficoColumnasFrm()
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

		#region Windows Form Designer generated code
		/// <summary>
		/// Método necesario para admitir el Diseñador, no se puede modificar
		/// el contenido del método con el editor de código.
		/// </summary>
		private void InitializeComponent()
		{
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GraficoColumnasFrm));
            this.btnGuia = new System.Windows.Forms.Button();
            this.btnSalir = new System.Windows.Forms.Button();
            this.btnAbrir = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // btnGuia
            // 
            this.btnGuia.BackColor = System.Drawing.Color.LightSalmon;
            this.btnGuia.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnGuia.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnGuia.Image = ((System.Drawing.Image)(resources.GetObject("btnGuia.Image")));
            this.btnGuia.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnGuia.Location = new System.Drawing.Point(553, 324);
            this.btnGuia.Name = "btnGuia";
            this.btnGuia.Size = new System.Drawing.Size(80, 24);
            this.btnGuia.TabIndex = 11;
            this.btnGuia.Text = "Guía";
            this.btnGuia.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnGuia.UseVisualStyleBackColor = false;
            this.btnGuia.Visible = false;
            this.btnGuia.Click += new System.EventHandler(this.btnGuia_Click);
            // 
            // btnSalir
            // 
            this.btnSalir.BackColor = System.Drawing.Color.DarkSalmon;
            this.btnSalir.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnSalir.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSalir.Image = ((System.Drawing.Image)(resources.GetObject("btnSalir.Image")));
            this.btnSalir.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSalir.Location = new System.Drawing.Point(465, 324);
            this.btnSalir.Name = "btnSalir";
            this.btnSalir.Size = new System.Drawing.Size(82, 24);
            this.btnSalir.TabIndex = 10;
            this.btnSalir.Text = "Cancelar";
            this.btnSalir.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnSalir.UseVisualStyleBackColor = false;
            this.btnSalir.Click += new System.EventHandler(this.btnSalir_Click);
            // 
            // btnAbrir
            // 
            this.btnAbrir.BackColor = System.Drawing.Color.DarkSalmon;
            this.btnAbrir.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnAbrir.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAbrir.Image = ((System.Drawing.Image)(resources.GetObject("btnAbrir.Image")));
            this.btnAbrir.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnAbrir.Location = new System.Drawing.Point(339, 324);
            this.btnAbrir.Name = "btnAbrir";
            this.btnAbrir.Size = new System.Drawing.Size(120, 24);
            this.btnAbrir.TabIndex = 9;
            this.btnAbrir.Text = "Abrir combinación";
            this.btnAbrir.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnAbrir.UseVisualStyleBackColor = false;
            this.btnAbrir.Click += new System.EventHandler(this.btnAbrir_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(880, 240);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(16, 8);
            this.pictureBox1.TabIndex = 1;
            this.pictureBox1.TabStop = false;
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.Maroon;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(231, 21);
            this.label1.TabIndex = 142;
            this.label1.Text = "Representación de las columnas";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // GraficoColumnasFrm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(972, 366);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnGuia);
            this.Controls.Add(this.btnSalir);
            this.Controls.Add(this.btnAbrir);
            this.Controls.Add(this.pictureBox1);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(988, 402);
            this.MinimumSize = new System.Drawing.Size(988, 402);
            this.Name = "GraficoColumnasFrm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "Representación gráfica de la combinación.";
            this.Load += new System.EventHandler(this.GraficoColumnasFrm_Load);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.GraficoColumnasFrm_Paint);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

		}
		#endregion

	    private static string deBase10(int numero, int nBase, int longitud)
		{
			if(nBase>10 || nBase <2)
			{
				MessageBox.Show("Error en la conversión de base.","Error",MessageBoxButtons.OK,MessageBoxIcon.Stop);
				return null;
			}
			string num="";
			int resto;
			int divisor=0;
			
			// Para nº inferior a la base que no pasa por el bucle ...
			if (numero<nBase)
			{
				resto=numero%nBase;
				num=resto.ToString();
			}
			if (numero==nBase)
			{
				divisor=numero/nBase;
			}
			
			while (numero>=nBase)
			{
				resto=numero%nBase;
				divisor=numero/nBase;
				num=resto + num;
				numero=divisor;
			}
			num=divisor + num;
			while (num.Length<longitud)
			{
				num="0" + num;
			}
			return num;
		}

		private void GraficoColumnasFrm_Paint(object sender, PaintEventArgs e)
		{
			Pen marco = new Pen(Color.Maroon, 1);

			if(!inicio)
			{
				return;
			}
			OpenFileDialog abreFicheroDialog = new OpenFileDialog();
			abreFicheroDialog.InitialDirectory = "Columnas\\" ;
			abreFicheroDialog.Filter = "Columnas(*.txt)|*.txt|Todos (*.*)|*.*" ;
			if(abreFicheroDialog.ShowDialog() == DialogResult.OK)
			{	
                //dibuja los marcos
                for (int y = 0; y < 10; y++)
                {
                    e.Graphics.DrawRectangle(marco, 9, (y * 25) + 50, 959, 25);
                }
                int alto;
                double escala;
			    para = false;
				inicio=false;
				Cursor=Cursors.WaitCursor;
				btnAbrir.Visible=false;
				btnSalir.Visible=false;
				btnGuia.Visible=false;
				string archivoEntrada = abreFicheroDialog.FileName;		    	
				// Abre el fichero y pasa las columnas a un array
				// Rellenamos una matriz con las columnas.
                IArchivoColumnas archComb = new ArchivoColumnasTexto(archivoEntrada);
				// Calcula las apuestas que contiene el fichero
				int[] matrizColumnas = archComb.LeerTodasColsANumero();
				int apuestas = matrizColumnas.Length;
				archComb.Cerrar();

				// Buscamos los límites inferior y superior de la combinación
				Array.Sort(matrizColumnas);
				minimo=matrizColumnas[0];
				maximo=matrizColumnas[apuestas-1];
				int diferencia = maximo-minimo;
				Pen myPen = new Pen(Color.BlueViolet, 2);

				if(diferencia<9566)
				{
					// Para un ancho de 1, hay menos columnas que lineas podemos representar (960*10).
					// El exceso se pinta en rojo para delimitar la zona.
					escala=1;
					alto=Convert.ToInt16((diferencia*10)/9580);
					SolidBrush relleno = new SolidBrush(Color.Maroon );
					e.Graphics.FillRectangle(relleno,maximo+1,(alto*25)+50,958-maximo,25);
					for(int i=alto+1;i<10;i++)
					{
						e.Graphics.FillRectangle(relleno,9,(i*25)+50,958,25);
					}
				}
				else
				{
					escala=diferencia/9565.938;
				}

				for(int i=0;i<apuestas;i++)
				{
                    if (!para)
                    {
                        double num = (matrizColumnas[i] - minimo)/escala;
                        // Dividimos en 10 barras 4782969/(500*957)=10
                        alto = Convert.ToInt16((num/958) - 0.5);
                        num -= (alto*958);
                        int ancho = Convert.ToInt16(num);
                        e.Graphics.DrawLine(myPen, ancho + 10, (alto*25) + 51, ancho + 10, (alto*25) + 74);
                    }
                    else
                    {
                        break;
                    }
				}
				btnAbrir.Visible=true;
				btnSalir.Visible=true;
				btnGuia.Visible=true;
				Cursor=Cursors.Default;
			}
		}

		private void btnSalir_Click(object sender, EventArgs e)
		{
		    para = true;
		}

		private void btnAbrir_Click(object sender, EventArgs e)
		{
			inicio=true;
			Refresh();
		}

		private void btnGuia_Click(object sender, EventArgs e)
		{
			string txt="";
		    byte[] sNL=new Byte[1];
		    sNL[0]=13;
		    string NL = System.Text.Encoding.ASCII.GetString(sNL);
			int diferencia = maximo-minimo;
			for(int i=1;i<11;i++)
			{
				string tmp = deBase10(((i-1)*(diferencia/10))+minimo,3,14);
				tmp=tmp.Replace("1","X");
				tmp=tmp.Replace("0","1");
				txt+=i.ToString("00")+")  "+tmp+NL;
			}
			MessageBox.Show(txt,"Filas representadas",MessageBoxButtons.OK,MessageBoxIcon.Information);
		}

		private void GraficoColumnasFrm_Load(object sender, EventArgs e)
		{
		}

		protected void dibuja()
		{
			Bitmap B=new Bitmap(pictureBox1.Width,pictureBox1.Height,System.Drawing.Imaging.PixelFormat.Format24bppRgb);
			Graphics lienzo = Graphics.FromImage(B);

			Pen marco = new Pen(Color.Red, 2);
			//dibuja los marcos
			for(int y=0;y<10;y++)
			{
				lienzo.DrawRectangle(marco,9,(y*25)+50,959,25);
			}
			OpenFileDialog abreFicheroDialog = new OpenFileDialog();
			abreFicheroDialog.InitialDirectory = "Columnas\\" ;
			abreFicheroDialog.Filter = "Columnas(*.txt)|*.txt|Todos (*.*)|*.*" ;
			if(abreFicheroDialog.ShowDialog() == DialogResult.OK)
			{
			    int alto;
			    double escala;
				inicio=false;
				Cursor=Cursors.WaitCursor;
				btnAbrir.Visible=false;
				btnSalir.Visible=false;
				btnGuia.Visible=false;
				string archivoEntrada = abreFicheroDialog.FileName;		    	
				// Abre el fichero y pasa las columnas a un array
				// Rellenamos una matriz con las columnas.
                IArchivoColumnas archComb = new ArchivoColumnasTexto(archivoEntrada);
				// Calcula las apuestas que contiene el fichero
				int[] matrizColumnas = archComb.LeerTodasColsANumero();
				int apuestas = matrizColumnas.Length;
				archComb.Cerrar();

				// Buscamos los límites inferior y superior de la combinación
				Array.Sort(matrizColumnas);
				minimo=matrizColumnas[0];
				maximo=matrizColumnas[apuestas-1];
				int diferencia = maximo-minimo;
				Pen myPen = new Pen(Color.BlueViolet, 2);
				if(diferencia<9566)
				{
					// Para un ancho de 1, hay menos columnas que lineas podemos representar (960*10).
					// El exceso se pinta en rojo para delimitar la zona.
					escala=1;
					alto=Convert.ToInt16((diferencia*10)/9580);
					SolidBrush relleno = new SolidBrush(Color.Red );
					lienzo.FillRectangle(relleno,maximo+1,(alto*25)+50,958-maximo,25);
					for(int i=alto+1;i<10;i++)
					{
						lienzo.FillRectangle(relleno,9,(i*25)+50,958,25);
					}
				}
				else
				{
					escala=diferencia/9565.938;
				}

				for(int i=0;i<apuestas;i++)
				{
					double num = (matrizColumnas[i]-minimo)/escala;
					// Dividimos en 10 barras 4782969/(500*957)=10
					alto=Convert.ToInt16((num/958)-0.5);
					num-=(alto*958);
					int ancho = Convert.ToInt16(num);
					lienzo.DrawLine(myPen,ancho+10,(alto*25)+51,ancho+10,(alto*25)+74);
				}
				btnAbrir.Visible=true;
				btnSalir.Visible=true;
				btnGuia.Visible=true;
				pictureBox1.Image=B;
				lienzo.Dispose();
				Cursor=Cursors.Default;
			}
		}
	}
}
