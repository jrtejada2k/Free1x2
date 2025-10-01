using System;
using System.IO;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using Free1X2.Utils;

namespace Free1X2.UI
{
	/// <summary>
	/// Summary description for HistoriaValoracionesFrm.
	/// </summary>
	public class HistoriaValoracionesFrm : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox txTemporada;
		private System.Windows.Forms.Button btTemporadaAnterior;
		private System.Windows.Forms.Button btTemporadaSiguiente;
		private System.Windows.Forms.Button btJornadaSiguiente;
		private System.Windows.Forms.Button btJornadaAnterior;
		private System.Windows.Forms.TextBox txJornada;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox txNombreFichero;
		private System.Windows.Forms.Label label3;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		
		public string archivoSalida;
		private int Temporada;
		private int Jornada;
		private Porcentajes Pct =new Porcentajes ();
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox txTemporada2;
		private System.Windows.Forms.Button btGuardar;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.Button button2;
		private System.Windows.Forms.Button btNuevo;
		private double[,] valores1X2= new double [14,3];

		public HistoriaValoracionesFrm(double [,] pv, int ptemporada, int pjornada, string pArchivoSalida)
		{
			InitializeComponent();
			valores1X2=pv;
			Temporada=ptemporada;
			Jornada=pjornada;
			archivoSalida=pArchivoSalida;
			txTemporada.Text = Temporada.ToString ();
			txTemporada2.Text = (Temporada+1).ToString ();
			txJornada.Text = Jornada.ToString ();
			if(archivoSalida !="") 
			{
				btGuardar.Enabled =true;
				txNombreFichero.Text = Path.GetFileName(archivoSalida);
			}
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HistoriaValoracionesFrm));
            this.label1 = new System.Windows.Forms.Label();
            this.txTemporada = new System.Windows.Forms.TextBox();
            this.btTemporadaAnterior = new System.Windows.Forms.Button();
            this.btTemporadaSiguiente = new System.Windows.Forms.Button();
            this.btJornadaSiguiente = new System.Windows.Forms.Button();
            this.btJornadaAnterior = new System.Windows.Forms.Button();
            this.txJornada = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txNombreFichero = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.txTemporada2 = new System.Windows.Forms.TextBox();
            this.btGuardar = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.btNuevo = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.label1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(223, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(112, 21);
            this.label1.TabIndex = 0;
            this.label1.Text = "de la temporada";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txTemporada
            // 
            this.txTemporada.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txTemporada.Location = new System.Drawing.Point(337, 16);
            this.txTemporada.Name = "txTemporada";
            this.txTemporada.Size = new System.Drawing.Size(35, 21);
            this.txTemporada.TabIndex = 1;
            this.txTemporada.Text = "2004";
            // 
            // btTemporadaAnterior
            // 
            this.btTemporadaAnterior.BackColor = System.Drawing.Color.LightSalmon;
            this.btTemporadaAnterior.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btTemporadaAnterior.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btTemporadaAnterior.Image = ((System.Drawing.Image)(resources.GetObject("btTemporadaAnterior.Image")));
            this.btTemporadaAnterior.Location = new System.Drawing.Point(420, 16);
            this.btTemporadaAnterior.Name = "btTemporadaAnterior";
            this.btTemporadaAnterior.Size = new System.Drawing.Size(20, 21);
            this.btTemporadaAnterior.TabIndex = 2;
            this.btTemporadaAnterior.UseVisualStyleBackColor = false;
            this.btTemporadaAnterior.Click += new System.EventHandler(this.btTemporadaAnterior_Click);
            // 
            // btTemporadaSiguiente
            // 
            this.btTemporadaSiguiente.BackColor = System.Drawing.Color.LightSalmon;
            this.btTemporadaSiguiente.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btTemporadaSiguiente.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btTemporadaSiguiente.Image = ((System.Drawing.Image)(resources.GetObject("btTemporadaSiguiente.Image")));
            this.btTemporadaSiguiente.Location = new System.Drawing.Point(441, 16);
            this.btTemporadaSiguiente.Name = "btTemporadaSiguiente";
            this.btTemporadaSiguiente.Size = new System.Drawing.Size(20, 21);
            this.btTemporadaSiguiente.TabIndex = 3;
            this.btTemporadaSiguiente.UseVisualStyleBackColor = false;
            this.btTemporadaSiguiente.Click += new System.EventHandler(this.btTemporadaSiguiente_Click);
            // 
            // btJornadaSiguiente
            // 
            this.btJornadaSiguiente.BackColor = System.Drawing.Color.LightSalmon;
            this.btJornadaSiguiente.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btJornadaSiguiente.Image = ((System.Drawing.Image)(resources.GetObject("btJornadaSiguiente.Image")));
            this.btJornadaSiguiente.Location = new System.Drawing.Point(203, 16);
            this.btJornadaSiguiente.Name = "btJornadaSiguiente";
            this.btJornadaSiguiente.Size = new System.Drawing.Size(20, 20);
            this.btJornadaSiguiente.TabIndex = 7;
            this.btJornadaSiguiente.UseVisualStyleBackColor = false;
            this.btJornadaSiguiente.Click += new System.EventHandler(this.btJornadaSiguiente_Click);
            // 
            // btJornadaAnterior
            // 
            this.btJornadaAnterior.BackColor = System.Drawing.Color.LightSalmon;
            this.btJornadaAnterior.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btJornadaAnterior.Image = ((System.Drawing.Image)(resources.GetObject("btJornadaAnterior.Image")));
            this.btJornadaAnterior.Location = new System.Drawing.Point(183, 16);
            this.btJornadaAnterior.Name = "btJornadaAnterior";
            this.btJornadaAnterior.Size = new System.Drawing.Size(20, 20);
            this.btJornadaAnterior.TabIndex = 6;
            this.btJornadaAnterior.UseVisualStyleBackColor = false;
            this.btJornadaAnterior.Click += new System.EventHandler(this.btJornadaAnterior_Click);
            // 
            // txJornada
            // 
            this.txJornada.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txJornada.Location = new System.Drawing.Point(158, 16);
            this.txJornada.Name = "txJornada";
            this.txJornada.Size = new System.Drawing.Size(24, 21);
            this.txJornada.TabIndex = 5;
            this.txJornada.Text = "1";
            // 
            // label2
            // 
            this.label2.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.label2.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(16, 16);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(141, 21);
            this.label2.TabIndex = 4;
            this.label2.Text = "Se guardará la jornada";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txNombreFichero
            // 
            this.txNombreFichero.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txNombreFichero.Location = new System.Drawing.Point(158, 38);
            this.txNombreFichero.Name = "txNombreFichero";
            this.txNombreFichero.Size = new System.Drawing.Size(228, 21);
            this.txNombreFichero.TabIndex = 8;
            // 
            // label3
            // 
            this.label3.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.label3.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(16, 38);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(141, 21);
            this.label3.TabIndex = 9;
            this.label3.Text = "en el fichero histórico";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label4
            // 
            this.label4.BackColor = System.Drawing.Color.Transparent;
            this.label4.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(372, 16);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(12, 21);
            this.label4.TabIndex = 12;
            this.label4.Text = "/";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // txTemporada2
            // 
            this.txTemporada2.BackColor = System.Drawing.Color.LemonChiffon;
            this.txTemporada2.Enabled = false;
            this.txTemporada2.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txTemporada2.Location = new System.Drawing.Point(384, 16);
            this.txTemporada2.Name = "txTemporada2";
            this.txTemporada2.Size = new System.Drawing.Size(35, 21);
            this.txTemporada2.TabIndex = 13;
            this.txTemporada2.Text = "2005";
            // 
            // btGuardar
            // 
            this.btGuardar.BackColor = System.Drawing.Color.DarkSalmon;
            this.btGuardar.Enabled = false;
            this.btGuardar.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btGuardar.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btGuardar.Image = ((System.Drawing.Image)(resources.GetObject("btGuardar.Image")));
            this.btGuardar.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btGuardar.Location = new System.Drawing.Point(122, 80);
            this.btGuardar.Name = "btGuardar";
            this.btGuardar.Size = new System.Drawing.Size(100, 32);
            this.btGuardar.TabIndex = 15;
            this.btGuardar.Text = "&Guardar";
            this.btGuardar.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btGuardar.UseVisualStyleBackColor = false;
            this.btGuardar.Click += new System.EventHandler(this.btGuardar_Click);
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.Color.LightSalmon;
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.button1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.Image = ((System.Drawing.Image)(resources.GetObject("button1.Image")));
            this.button1.Location = new System.Drawing.Point(387, 38);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(24, 21);
            this.button1.TabIndex = 16;
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.BackColor = System.Drawing.Color.DarkSalmon;
            this.button2.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.button2.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button2.Image = ((System.Drawing.Image)(resources.GetObject("button2.Image")));
            this.button2.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button2.Location = new System.Drawing.Point(246, 80);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(100, 32);
            this.button2.TabIndex = 17;
            this.button2.Text = "&Cancelar";
            this.button2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.button2.UseVisualStyleBackColor = false;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // btNuevo
            // 
            this.btNuevo.BackColor = System.Drawing.Color.LightSalmon;
            this.btNuevo.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btNuevo.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btNuevo.Image = ((System.Drawing.Image)(resources.GetObject("btNuevo.Image")));
            this.btNuevo.Location = new System.Drawing.Point(412, 38);
            this.btNuevo.Name = "btNuevo";
            this.btNuevo.Size = new System.Drawing.Size(24, 21);
            this.btNuevo.TabIndex = 18;
            this.btNuevo.UseVisualStyleBackColor = false;
            this.btNuevo.Click += new System.EventHandler(this.btNuevo_Click);
            // 
            // HistoriaValoracionesFrm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.Color.Bisque;
            this.ClientSize = new System.Drawing.Size(468, 122);
            this.Controls.Add(this.btNuevo);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.btGuardar);
            this.Controls.Add(this.txTemporada2);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txNombreFichero);
            this.Controls.Add(this.btJornadaSiguiente);
            this.Controls.Add(this.btJornadaAnterior);
            this.Controls.Add(this.txJornada);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btTemporadaSiguiente);
            this.Controls.Add(this.btTemporadaAnterior);
            this.Controls.Add(this.txTemporada);
            this.Controls.Add(this.label1);
            this.Name = "HistoriaValoracionesFrm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Valoraciones históricas";
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		private void btSeleccionarFichero_Click(object sender, System.EventArgs e)
		{
			OpenFileDialog abreFiltroDialog = new OpenFileDialog ();
			abreFiltroDialog.InitialDirectory = "Columnas\\" ;
			abreFiltroDialog.Filter = "Valoraciones históricas(*.txt)|*.txt|Todos los archivos (*.*)|*.*" ;
			if(abreFiltroDialog.ShowDialog () == DialogResult.OK)
			{		    	
				archivoSalida = abreFiltroDialog.FileName;		    	
				txNombreFichero.Text = Path.GetFileName(archivoSalida);
				string tempo=Temporada.ToString() + "/" + (++Temporada).ToString();
				string jorna =Jornada.ToString ().PadLeft (2,'0');
				Pct.GuardarValoraciones (archivoSalida,(char) 9,valores1X2,tempo,jorna);
				Close ();
			}
		}

		private void btTemporadaAnterior_Click(object sender, System.EventArgs e)
		{
			txTemporada2.Text=txTemporada.Text ;
			Temporada--;
			txTemporada.Text=Temporada.ToString();
		}

		private void btTemporadaSiguiente_Click(object sender, System.EventArgs e)
		{
			
			Temporada++;
			txTemporada.Text=Temporada.ToString();
			txTemporada2.Text=(Temporada+1).ToString();
		}

		private void btJornadaAnterior_Click(object sender, System.EventArgs e)
		{
			if(Jornada>0)
			{
				Jornada--;
				txJornada.Text = Jornada.ToString();
			}
		}

		private void btJornadaSiguiente_Click(object sender, System.EventArgs e)
		{
			if(Jornada<43)
			{
				Jornada++;
				txJornada.Text = Jornada.ToString();
			}
		}

		private void btCrearNuevo_Click(object sender, System.EventArgs e)
		{
			SaveFileDialog abreFiltroDialog = new SaveFileDialog ();
			abreFiltroDialog.InitialDirectory = "Columnas\\" ;
			abreFiltroDialog.Filter = "Valoraciones históricas(*.txt)|*.txt|Todos los archivos (*.*)|*.*" ;
			if(abreFiltroDialog.ShowDialog () == DialogResult.OK)
			{		    	
				archivoSalida = abreFiltroDialog.FileName;		    	
				txNombreFichero.Text = Path.GetFileName(archivoSalida);
				string tempo=Temporada.ToString() + "/" + (++Temporada).ToString();
				string jorna =Jornada.ToString ().PadLeft (2,'0');
				Pct.GuardarValoraciones (archivoSalida,(char) 9,valores1X2,tempo,jorna);
				Close ();
			}
		}

		private void btGuardar_Click(object sender, System.EventArgs e)
		{
			if(archivoSalida !="")
			{		   
				string tempo=Temporada.ToString() + "/" + (++Temporada).ToString();
				string jorna =Jornada.ToString ().PadLeft (2,'0');
				Pct.GuardarValoraciones (archivoSalida,(char) 9,valores1X2,tempo,jorna);
				Close ();
			}
		}

		private void button1_Click(object sender, System.EventArgs e)
		{
			OpenFileDialog abreFiltroDialog = new OpenFileDialog ();
			abreFiltroDialog.InitialDirectory = "Columnas\\" ;
			abreFiltroDialog.Filter = "Valoraciones históricas(*.txt)|*.txt|Todos los archivos (*.*)|*.*" ;
			if(abreFiltroDialog.ShowDialog () == DialogResult.OK)
			{		    	
				archivoSalida = abreFiltroDialog.FileName;		    	
				txNombreFichero.Text = Path.GetFileName(archivoSalida);
				btGuardar.Enabled =true;
			}
		}

		private void button2_Click(object sender, System.EventArgs e)
		{
			Close ();
		}

		private void btNuevo_Click(object sender, System.EventArgs e)
		{
			SaveFileDialog abreFiltroDialog = new SaveFileDialog ();
			abreFiltroDialog.InitialDirectory = "Columnas\\" ;
			abreFiltroDialog.Filter = "Valoraciones históricas(*.txt)|*.txt|Todos los archivos (*.*)|*.*" ;
			if(abreFiltroDialog.ShowDialog () == DialogResult.OK)
			{		    	
				archivoSalida = abreFiltroDialog.FileName;		    	
				txNombreFichero.Text = Path.GetFileName(archivoSalida);
				btGuardar.Enabled =true;
			}
		}

	}
}
