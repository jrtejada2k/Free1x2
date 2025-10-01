using System;
using System.Windows.Forms;
using Free1X2.EntradaSalida;

namespace Free1X2.UI
{
	/// <summary>
	/// Descripción breve de Form1.
	/// </summary>
	public class GeneradorCPSDiferencias : Form
	{
		private Label label1;
		private Label label2;
		private Label label3;
		private Button button1;
		private Button button2;
		private Button button3;
		private TextBox txtColumna1;
		private TextBox txtColumna2;
		private TextBox txtArchivo;
		private SaveFileDialog fd;
		protected string archivo="";
		private GroupBox groupBox1;
		private RadioButton rDobles;
		private RadioButton rFijos;
		private GroupBox groupBox2;
		private RadioButton rDif2;
		private RadioButton rDif1;

	    /// <summary>
		/// Variable del diseñador requerida.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public GeneradorCPSDiferencias()
		{
			InitializeComponent();
            FormulariosHelper fH = new FormulariosHelper();
            fH.Traducir(this);
		}

		/// <summary>
		/// Limpiar los recursos que se estén utilizando.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GeneradorCPSDiferencias));
            this.label1 = new System.Windows.Forms.Label();
            this.txtColumna1 = new System.Windows.Forms.TextBox();
            this.txtColumna2 = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.txtArchivo = new System.Windows.Forms.TextBox();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.fd = new System.Windows.Forms.SaveFileDialog();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rDobles = new System.Windows.Forms.RadioButton();
            this.rFijos = new System.Windows.Forms.RadioButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.rDif2 = new System.Windows.Forms.RadioButton();
            this.rDif1 = new System.Windows.Forms.RadioButton();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(7, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(97, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "Columna Inicial:";
            // 
            // txtColumna1
            // 
            this.txtColumna1.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtColumna1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtColumna1.Location = new System.Drawing.Point(128, 24);
            this.txtColumna1.MaxLength = 14;
            this.txtColumna1.Name = "txtColumna1";
            this.txtColumna1.Size = new System.Drawing.Size(112, 21);
            this.txtColumna1.TabIndex = 1;
            // 
            // txtColumna2
            // 
            this.txtColumna2.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtColumna2.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtColumna2.Location = new System.Drawing.Point(128, 56);
            this.txtColumna2.MaxLength = 14;
            this.txtColumna2.Name = "txtColumna2";
            this.txtColumna2.Size = new System.Drawing.Size(112, 21);
            this.txtColumna2.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(7, 56);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(121, 16);
            this.label2.TabIndex = 2;
            this.label2.Text = "Columna alternativa:";
            // 
            // label3
            // 
            this.label3.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(4, 88);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(100, 16);
            this.label3.TabIndex = 4;
            this.label3.Text = "Archivo destino";
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.Color.LightSalmon;
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.button1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.Image = ((System.Drawing.Image)(resources.GetObject("button1.Image")));
            this.button1.Location = new System.Drawing.Point(103, 88);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(24, 24);
            this.button1.TabIndex = 6;
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // txtArchivo
            // 
            this.txtArchivo.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtArchivo.Location = new System.Drawing.Point(128, 88);
            this.txtArchivo.Multiline = true;
            this.txtArchivo.Name = "txtArchivo";
            this.txtArchivo.Size = new System.Drawing.Size(152, 48);
            this.txtArchivo.TabIndex = 7;
            // 
            // button2
            // 
            this.button2.BackColor = System.Drawing.Color.DarkSalmon;
            this.button2.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.button2.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button2.Image = ((System.Drawing.Image)(resources.GetObject("button2.Image")));
            this.button2.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button2.Location = new System.Drawing.Point(38, 240);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(100, 32);
            this.button2.TabIndex = 10;
            this.button2.Text = "Generar";
            this.button2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.button2.UseVisualStyleBackColor = false;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.BackColor = System.Drawing.Color.DarkSalmon;
            this.button3.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.button3.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button3.Image = ((System.Drawing.Image)(resources.GetObject("button3.Image")));
            this.button3.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button3.Location = new System.Drawing.Point(158, 240);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(100, 32);
            this.button3.TabIndex = 11;
            this.button3.Text = "Cancelar";
            this.button3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.button3.UseVisualStyleBackColor = false;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // fd
            // 
            this.fd.FileName = "doc1";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rDobles);
            this.groupBox1.Controls.Add(this.rFijos);
            this.groupBox1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.ForeColor = System.Drawing.Color.Maroon;
            this.groupBox1.Location = new System.Drawing.Point(24, 152);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(88, 64);
            this.groupBox1.TabIndex = 12;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Columnas";
            // 
            // rDobles
            // 
            this.rDobles.Checked = true;
            this.rDobles.Location = new System.Drawing.Point(16, 40);
            this.rDobles.Name = "rDobles";
            this.rDobles.Size = new System.Drawing.Size(64, 16);
            this.rDobles.TabIndex = 11;
            this.rDobles.TabStop = true;
            this.rDobles.Text = "Dobles";
            // 
            // rFijos
            // 
            this.rFijos.Location = new System.Drawing.Point(16, 16);
            this.rFijos.Name = "rFijos";
            this.rFijos.Size = new System.Drawing.Size(48, 16);
            this.rFijos.TabIndex = 10;
            this.rFijos.Text = "Fijos";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.rDif2);
            this.groupBox2.Controls.Add(this.rDif1);
            this.groupBox2.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox2.ForeColor = System.Drawing.Color.Maroon;
            this.groupBox2.Location = new System.Drawing.Point(168, 152);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(88, 64);
            this.groupBox2.TabIndex = 13;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Diferencias";
            // 
            // rDif2
            // 
            this.rDif2.Checked = true;
            this.rDif2.Location = new System.Drawing.Point(16, 40);
            this.rDif2.Name = "rDif2";
            this.rDif2.Size = new System.Drawing.Size(32, 16);
            this.rDif2.TabIndex = 11;
            this.rDif2.TabStop = true;
            this.rDif2.Text = "2";
            // 
            // rDif1
            // 
            this.rDif1.Location = new System.Drawing.Point(16, 16);
            this.rDif1.Name = "rDif1";
            this.rDif1.Size = new System.Drawing.Size(32, 16);
            this.rDif1.TabIndex = 10;
            this.rDif1.Text = "1";
            // 
            // GeneradorCPSDiferencias
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.Color.Bisque;
            this.ClientSize = new System.Drawing.Size(296, 294);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.txtArchivo);
            this.Controls.Add(this.txtColumna2);
            this.Controls.Add(this.txtColumna1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "GeneradorCPSDiferencias";
            this.Text = "Generador de CPs por diferencias";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		/// <summary>
		/// </summary>

		private void button3_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void button1_Click(object sender, EventArgs e)
		{
			// Busca el archivo de destino
			fd.ShowDialog();
			archivo=fd.FileName;
			txtArchivo.Text=archivo;

		}

		private void button2_Click(object sender, EventArgs e)
		{
		    string[] columnas=null;
            IArchivoColumnas f = new ArchivoColumnasTexto(archivo);
			string cInicial = txtColumna1.Text.ToUpper();
			string cAlterna = txtColumna2.Text.ToUpper();
			// Comprueba la validez de las columnas
			if(esValida(cInicial)==false)
			{
				MessageBox.Show("La cadena inicial no es válida");
				return;
			}
			if(esValida(cAlterna)==false)
			{
				MessageBox.Show("La cadena alternativa no es válida");
				return;
			}
			if(!(archivo.Length>0))
			{
				MessageBox.Show("No se ha indicado el fichero.");
				return;
			}
			if(rFijos.Checked)
			{
				if(rDif1.Checked)
					columnas=combinarFijos1(cInicial,cAlterna);
				else if(rDif2.Checked)
					columnas=combinarFijos2(cInicial,cAlterna);
			}
			else
			{
				if(rDif1.Checked)
					columnas=combinarDobles1(cInicial,cAlterna);
				else if(rDif2.Checked)
					columnas=combinarDobles2(cInicial,cAlterna);
			}

			// Ahora solo tendríamos que guardar las columnas de la matriz a un archivo.
			for(int i=0;i<columnas.Length;i++)
			{
				f.GuardarColsComa(columnas[i]);
			}
			f.Cerrar();
			MessageBox.Show("Columnas creadas");
		}

		private bool esValida(string cadena)
		{
			string valido="1X2";
			if(cadena.Length!=14)
			{
				return false;
			}
			for(int i=0;i<14;i++)
			{
				if(Array.IndexOf(valido.ToCharArray(),cadena[i])<0)
				{
					return false;
				}
			}
			return true;
		}

		private string[] combinarFijos1(string c1, string c2)
		{
			string[] cadena=new string[15];
		    for(int i=0;i<15;i++)
			{
				string tmp = "";
				for(int j=0;j<14;j++)
				{
					if(j==i)
					{
						tmp+=c2.Substring(j,1)+",";
					}
					else
					{
						tmp+=c1.Substring(j,1)+",";
					}
				}
				cadena[i]=tmp.Substring(0,tmp.Length-1);
			}
			return cadena;
		}

		private string[] combinarFijos2(string c1, string c2)
		{
			string[] cadena=new string[92];
			int n=0;
			string tmp="";
			// Añadimos la cadena inicial
			for(int i=0;i<14;i++)
			{
				tmp+=c1.Substring(i,1)+",";
			}
			cadena[n]=tmp.Substring(0,tmp.Length-1);
			for(int i=0;i<15;i++)
			{
				for(int j=i+1;j<14;j++)
				{
					n++;
					tmp="";
					for(int k=0;k<14;k++)
					{
						if(k==i || k==j)
						{
							tmp+=c2.Substring(k,1)+",";
						}
						else
						{
							tmp+=c1.Substring(k,1)+",";
						}
					}
					// Al asignar a la matriz, quitamos la ","
					cadena[n]=tmp.Substring(0,tmp.Length-1);
				}
			}
			return cadena;
		}

		private string[] combinarDobles1(string c1, string c2)
		{
			string valido="1X2";
			string cM="";
			string[] cadena=new string[15];

		    // Establecemos la cadena intermedia
			for(int i=0;i<14;i++)
			{
			    for(int j=0;j<3;j++)
				{
					if((c1.Substring(i,1)!=valido.Substring(j,1)) && (c2.Substring(i,1)!=valido.Substring(j,1)))
					{
						cM+=valido.Substring(j,1);
					}
				}
			}
			for(int i=0;i<15;i++)
			{
				string tmp = "";
				for(int j=0;j<14;j++)
				{
					if(j==i)
					{
						tmp+=c2.Substring(j,1)+cM.Substring(j,1)+",";
					}
					else
					{
						tmp+=c1.Substring(j,1)+cM.Substring(j,1)+",";
					}
				}
				// Al asignar a la matriz, quitamos la ","
				cadena[i]=tmp.Substring(0,tmp.Length-1);
			}
			return cadena;
		}

		private string[] combinarDobles2(string c1, string c2)
		{
			string valido="1X2";
			string cM="";
			string[] cadena=new string[92];
			int n=0;

		    // Establecemos la cadena intermedia
			for(int i=0;i<14;i++)
			{
			    for(int j=0;j<3;j++)
				{
					if((c1.Substring(i,1)!=valido.Substring(j,1)) && (c2.Substring(i,1)!=valido.Substring(j,1)))
					{
						cM+=valido.Substring(j,1);
					}
				}
			}
			
			// Añadimos la cadena inicial
			string tmp = "";
			for(int k=0;k<14;k++)
			{
				tmp+=c1.Substring(k,1)+cM.Substring(k,1)+",";
			}
			// Al asignar a la matriz, quitamos la ","
			cadena[n]=tmp.Substring(0,tmp.Length-1);

			for(int i=0;i<15;i++)
			{
				for(int j=i+1;j<14;j++)
				{
					n++;
					tmp="";
					for(int k=0;k<14;k++)
					{
						if(k==i || k==j)
						{
							tmp+=c2.Substring(k,1)+cM.Substring(k,1)+",";
						}
						else
						{
							tmp+=c1.Substring(k,1)+cM.Substring(k,1)+",";
						}
					}
					// Al asignar a la matriz, quitamos la ","
					cadena[n]=tmp.Substring(0,tmp.Length-1);
				}
			}
			return cadena;
		}

		private void Form1_Load(object sender, EventArgs e)
		{
			fd.AddExtension=true;
			fd.CheckPathExists=true;
			fd.DefaultExt=".txt";
			fd.Filter="Archivos de texto (*.txt)|*.txt|Todos los archivos (*.*)|*.*";
			fd.FilterIndex=0;
			fd.OverwritePrompt=true;
			fd.Title="Guardar archivo";
			fd.FileName="";
		}

	}
}
