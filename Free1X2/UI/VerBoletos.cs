using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.IO;
using Free1X2.EntradaSalida;

namespace Free1X2.UI
{
	/// <summary>
	/// Descripción breve de VerBoletos.
	/// </summary>
	public class VerBoletos : System.Windows.Forms.Form
	{
		protected BoletoFrm boleto;
		protected OrdenarMatriz ordenarPor=new OrdenarMatriz();
		protected TipoOrden tipoOrden=new TipoOrden();
		private System.Windows.Forms.Button abrir;
		private System.Windows.Forms.TextBox fichero;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.Button btnOk;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.RadioButton o0;
		private System.Windows.Forms.RadioButton t1;
		private System.Windows.Forms.RadioButton o1;
		private System.Windows.Forms.RadioButton o2;
		private System.Windows.Forms.RadioButton o3;
		private System.Windows.Forms.RadioButton o5;
		private System.Windows.Forms.RadioButton o4;
		private System.Windows.Forms.RadioButton t2;
		/// <summary>
		/// Variable del diseñador requerida.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public VerBoletos()
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(VerBoletos));
            this.abrir = new System.Windows.Forms.Button();
            this.fichero = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.o5 = new System.Windows.Forms.RadioButton();
            this.o4 = new System.Windows.Forms.RadioButton();
            this.o3 = new System.Windows.Forms.RadioButton();
            this.o2 = new System.Windows.Forms.RadioButton();
            this.o1 = new System.Windows.Forms.RadioButton();
            this.o0 = new System.Windows.Forms.RadioButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.t2 = new System.Windows.Forms.RadioButton();
            this.t1 = new System.Windows.Forms.RadioButton();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // abrir
            // 
            this.abrir.BackColor = System.Drawing.Color.LightSalmon;
            this.abrir.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.abrir.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.abrir.Image = ((System.Drawing.Image)(resources.GetObject("abrir.Image")));
            this.abrir.Location = new System.Drawing.Point(344, 8);
            this.abrir.Name = "abrir";
            this.abrir.Size = new System.Drawing.Size(24, 32);
            this.abrir.TabIndex = 11;
            this.abrir.UseVisualStyleBackColor = false;
            this.abrir.Click += new System.EventHandler(this.abrir_Click);
            // 
            // fichero
            // 
            this.fichero.BackColor = System.Drawing.SystemColors.Control;
            this.fichero.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.fichero.Location = new System.Drawing.Point(15, 8);
            this.fichero.Multiline = true;
            this.fichero.Name = "fichero";
            this.fichero.ReadOnly = true;
            this.fichero.Size = new System.Drawing.Size(328, 32);
            this.fichero.TabIndex = 10;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.o5);
            this.groupBox1.Controls.Add(this.o4);
            this.groupBox1.Controls.Add(this.o3);
            this.groupBox1.Controls.Add(this.o2);
            this.groupBox1.Controls.Add(this.o1);
            this.groupBox1.Controls.Add(this.o0);
            this.groupBox1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.ForeColor = System.Drawing.Color.Maroon;
            this.groupBox1.Location = new System.Drawing.Point(16, 56);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(352, 72);
            this.groupBox1.TabIndex = 12;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Ordenar por ...";
            // 
            // o5
            // 
            this.o5.Enabled = false;
            this.o5.ForeColor = System.Drawing.Color.Black;
            this.o5.Location = new System.Drawing.Point(134, 44);
            this.o5.Name = "o5";
            this.o5.Size = new System.Drawing.Size(120, 20);
            this.o5.TabIndex = 5;
            this.o5.Text = "Signos seguidos";
            this.o5.CheckedChanged += new System.EventHandler(this.o5_CheckedChanged);
            // 
            // o4
            // 
            this.o4.ForeColor = System.Drawing.Color.Black;
            this.o4.Location = new System.Drawing.Point(8, 46);
            this.o4.Name = "o4";
            this.o4.Size = new System.Drawing.Size(120, 16);
            this.o4.TabIndex = 4;
            this.o4.Text = "Interrupciones";
            this.o4.CheckedChanged += new System.EventHandler(this.o4_CheckedChanged);
            // 
            // o3
            // 
            this.o3.ForeColor = System.Drawing.Color.Black;
            this.o3.Location = new System.Drawing.Point(295, 24);
            this.o3.Name = "o3";
            this.o3.Size = new System.Drawing.Size(32, 16);
            this.o3.TabIndex = 3;
            this.o3.Text = "2";
            this.o3.CheckedChanged += new System.EventHandler(this.o3_CheckedChanged);
            // 
            // o2
            // 
            this.o2.ForeColor = System.Drawing.Color.Black;
            this.o2.Location = new System.Drawing.Point(241, 24);
            this.o2.Name = "o2";
            this.o2.Size = new System.Drawing.Size(32, 16);
            this.o2.TabIndex = 2;
            this.o2.Text = "X";
            this.o2.CheckedChanged += new System.EventHandler(this.o2_CheckedChanged);
            // 
            // o1
            // 
            this.o1.ForeColor = System.Drawing.Color.Black;
            this.o1.Location = new System.Drawing.Point(134, 24);
            this.o1.Name = "o1";
            this.o1.Size = new System.Drawing.Size(96, 16);
            this.o1.TabIndex = 1;
            this.o1.Text = "Variantes";
            this.o1.CheckedChanged += new System.EventHandler(this.o1_CheckedChanged);
            // 
            // o0
            // 
            this.o0.Checked = true;
            this.o0.ForeColor = System.Drawing.Color.Black;
            this.o0.Location = new System.Drawing.Point(8, 24);
            this.o0.Name = "o0";
            this.o0.Size = new System.Drawing.Size(108, 16);
            this.o0.TabIndex = 0;
            this.o0.TabStop = true;
            this.o0.Text = "No ordenar";
            this.o0.CheckedChanged += new System.EventHandler(this.o0_CheckedChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.t2);
            this.groupBox2.Controls.Add(this.t1);
            this.groupBox2.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox2.ForeColor = System.Drawing.Color.Maroon;
            this.groupBox2.Location = new System.Drawing.Point(16, 136);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(352, 50);
            this.groupBox2.TabIndex = 13;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Tipo de ordenamiento";
            // 
            // t2
            // 
            this.t2.ForeColor = System.Drawing.Color.Black;
            this.t2.Location = new System.Drawing.Point(122, 17);
            this.t2.Name = "t2";
            this.t2.Size = new System.Drawing.Size(102, 16);
            this.t2.TabIndex = 1;
            this.t2.Text = "descendente";
            this.t2.CheckedChanged += new System.EventHandler(this.t2_CheckedChanged);
            // 
            // t1
            // 
            this.t1.Checked = true;
            this.t1.ForeColor = System.Drawing.Color.Black;
            this.t1.Location = new System.Drawing.Point(8, 17);
            this.t1.Name = "t1";
            this.t1.Size = new System.Drawing.Size(108, 16);
            this.t1.TabIndex = 0;
            this.t1.TabStop = true;
            this.t1.Text = "ascendente";
            this.t1.CheckedChanged += new System.EventHandler(this.t1_CheckedChanged);
            // 
            // btnOk
            // 
            this.btnOk.BackColor = System.Drawing.Color.DarkSalmon;
            this.btnOk.Enabled = false;
            this.btnOk.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnOk.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnOk.Image = ((System.Drawing.Image)(resources.GetObject("btnOk.Image")));
            this.btnOk.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnOk.Location = new System.Drawing.Point(53, 192);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(128, 26);
            this.btnOk.TabIndex = 32;
            this.btnOk.Text = "Ok";
            this.btnOk.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnOk.UseVisualStyleBackColor = false;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.Color.DarkSalmon;
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnCancel.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancel.Image = ((System.Drawing.Image)(resources.GetObject("btnCancel.Image")));
            this.btnCancel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCancel.Location = new System.Drawing.Point(197, 192);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(128, 26);
            this.btnCancel.TabIndex = 31;
            this.btnCancel.Text = "Cancelar";
            this.btnCancel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // VerBoletos
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.Color.Bisque;
            this.ClientSize = new System.Drawing.Size(379, 230);
            this.ControlBox = false;
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.abrir);
            this.Controls.Add(this.fichero);
            this.Name = "VerBoletos";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "Ver Boletos";
            this.Load += new System.EventHandler(this.VerBoletos_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		private void abrir_Click(object sender, System.EventArgs e)
		{
			string archivoEntrada;
			OpenFileDialog abreFicheroDialog = new OpenFileDialog();

			abreFicheroDialog.InitialDirectory = "Columnas\\" ;
			abreFicheroDialog.Filter = "Columnas(*.txt)|*.txt|Columnas(*.cols)|*.cols|Todos (*.*)|*.*" ;
			if(abreFicheroDialog.ShowDialog() == DialogResult.OK)
			{		    	
				archivoEntrada = abreFicheroDialog.FileName;		    	
				//fichero.Text = Path.GetFileName(archivoEntrada);
                fichero.Text = archivoEntrada;
				btnOk.Enabled = true;
				boleto = new BoletoFrm();
				boleto.ArchivoCombinacion = fichero.Text;
				boleto.ordenarPor=ordenarPor;
				boleto.tipoOrden=tipoOrden;
			}
		}

		private void btnCancel_Click(object sender, System.EventArgs e)
		{
			this.Close();
		}

		private void btnOk_Click(object sender, System.EventArgs e)
		{
			if(boleto==null)
			{
				boleto = new BoletoFrm();
				boleto.ArchivoCombinacion = fichero.Text;
				boleto.ordenarPor=ordenarPor;
				boleto.tipoOrden=tipoOrden;
			}
			boleto.ShowDialog();
			boleto=null;
		}

		private void VerBoletos_Load(object sender, System.EventArgs e)
		{
			ordenarPor=OrdenarMatriz.Signo;
			tipoOrden=TipoOrden.asc;
			if(boleto!=null)
			{
				boleto.ordenarPor=ordenarPor;
				boleto.tipoOrden=tipoOrden;
			}
		}

		private void t1_CheckedChanged(object sender, System.EventArgs e)
		{
			tipoOrden=TipoOrden.asc;
			if(boleto!=null)
			{
				boleto.tipoOrden=tipoOrden;
			}
		}

		private void t2_CheckedChanged(object sender, System.EventArgs e)
		{
			tipoOrden=TipoOrden.desc;
			if(boleto!=null)
			{
				boleto.tipoOrden=tipoOrden;
			}
		}

		private void o0_CheckedChanged(object sender, System.EventArgs e)
		{
			ordenarPor=OrdenarMatriz.Signo;
			if(boleto!=null)
			{
				boleto.ordenarPor=ordenarPor;
			}
		}

		private void o1_CheckedChanged(object sender, System.EventArgs e)
		{
			ordenarPor=OrdenarMatriz.Variantes;
			if(boleto!=null)
			{
				boleto.ordenarPor=ordenarPor;
			}
		}

		private void o2_CheckedChanged(object sender, System.EventArgs e)
		{
			ordenarPor=OrdenarMatriz.Equis;
			if(boleto!=null)
			{
				boleto.ordenarPor=ordenarPor;
			}
		}

		private void o3_CheckedChanged(object sender, System.EventArgs e)
		{
			ordenarPor=OrdenarMatriz.Doses;
			if(boleto!=null)
			{
				boleto.ordenarPor=ordenarPor;
			}
		}

		private void o4_CheckedChanged(object sender, System.EventArgs e)
		{
			ordenarPor=OrdenarMatriz.Interrupciones;
			if(boleto!=null)
			{
				boleto.ordenarPor=ordenarPor;
			}
		}

		private void o5_CheckedChanged(object sender, System.EventArgs e)
		{
			ordenarPor=OrdenarMatriz.SignosSeguidos;
			if(boleto!=null)
			{
				boleto.ordenarPor=ordenarPor;
			}
		}
	}
}
