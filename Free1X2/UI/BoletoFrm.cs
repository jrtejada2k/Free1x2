using System;
using System.Windows.Forms;
using System.IO;
using Free1X2.EntradaSalida;
using Free1X2.UI.Controls;

namespace Free1X2.UI
{
	/// <summary>
	/// Descripción breve de BoletoFrm2.
	/// </summary>
	public class BoletoFrm : Form
	{
		protected int boletos;
		public OrdenarMatriz ordenarPor;
		public TipoOrden tipoOrden;
		private ControlBoleto controlBoleto1;
		private Panel panel1;
		private Button btnIr;
		private Button btnAnterior;
		private Button btnPrimero;
		private Button btnUltimo;
		private Button btnSiguiente;
		private Label label1;
		private Label totalBoletos;
		private Label label3;
		private TextBox boletoActual;
        public string ArchivoCombinacion;
		/// <summary>
		/// Variable del diseñador requerida.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public BoletoFrm()
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnIr = new System.Windows.Forms.Button();
            this.btnAnterior = new System.Windows.Forms.Button();
            this.btnPrimero = new System.Windows.Forms.Button();
            this.btnUltimo = new System.Windows.Forms.Button();
            this.btnSiguiente = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.totalBoletos = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.boletoActual = new System.Windows.Forms.TextBox();
            this.controlBoleto1 = new Free1X2.UI.Controls.ControlBoleto();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.Bisque;
            this.panel1.Controls.Add(this.btnIr);
            this.panel1.Controls.Add(this.btnAnterior);
            this.panel1.Controls.Add(this.btnPrimero);
            this.panel1.Controls.Add(this.btnUltimo);
            this.panel1.Controls.Add(this.btnSiguiente);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.totalBoletos);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.boletoActual);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(370, 32);
            this.panel1.TabIndex = 25;
            // 
            // btnIr
            // 
            this.btnIr.BackColor = System.Drawing.Color.DarkSalmon;
            this.btnIr.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnIr.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnIr.Location = new System.Drawing.Point(193, 8);
            this.btnIr.Name = "btnIr";
            this.btnIr.Size = new System.Drawing.Size(24, 21);
            this.btnIr.TabIndex = 28;
            this.btnIr.Text = "Ir";
            this.btnIr.UseVisualStyleBackColor = false;
            this.btnIr.Click += new System.EventHandler(this.btnIr_Click);
            // 
            // btnAnterior
            // 
            this.btnAnterior.BackColor = System.Drawing.Color.DarkSalmon;
            this.btnAnterior.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnAnterior.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAnterior.ForeColor = System.Drawing.Color.Maroon;
            this.btnAnterior.Location = new System.Drawing.Point(37, 8);
            this.btnAnterior.Name = "btnAnterior";
            this.btnAnterior.Size = new System.Drawing.Size(30, 21);
            this.btnAnterior.TabIndex = 27;
            this.btnAnterior.Text = "<";
            this.btnAnterior.UseVisualStyleBackColor = false;
            this.btnAnterior.Click += new System.EventHandler(this.btnAnterior_Click);
            // 
            // btnPrimero
            // 
            this.btnPrimero.BackColor = System.Drawing.Color.DarkSalmon;
            this.btnPrimero.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnPrimero.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnPrimero.ForeColor = System.Drawing.Color.Maroon;
            this.btnPrimero.Location = new System.Drawing.Point(6, 8);
            this.btnPrimero.Name = "btnPrimero";
            this.btnPrimero.Size = new System.Drawing.Size(30, 21);
            this.btnPrimero.TabIndex = 26;
            this.btnPrimero.Text = "|<";
            this.btnPrimero.UseVisualStyleBackColor = false;
            this.btnPrimero.Click += new System.EventHandler(this.btnPrimero_Click);
            // 
            // btnUltimo
            // 
            this.btnUltimo.BackColor = System.Drawing.Color.DarkSalmon;
            this.btnUltimo.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnUltimo.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnUltimo.ForeColor = System.Drawing.Color.Maroon;
            this.btnUltimo.Location = new System.Drawing.Point(335, 8);
            this.btnUltimo.Name = "btnUltimo";
            this.btnUltimo.Size = new System.Drawing.Size(30, 21);
            this.btnUltimo.TabIndex = 8;
            this.btnUltimo.Text = ">|";
            this.btnUltimo.UseVisualStyleBackColor = false;
            this.btnUltimo.Click += new System.EventHandler(this.btnUltimo_Click);
            // 
            // btnSiguiente
            // 
            this.btnSiguiente.BackColor = System.Drawing.Color.DarkSalmon;
            this.btnSiguiente.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnSiguiente.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSiguiente.ForeColor = System.Drawing.Color.Maroon;
            this.btnSiguiente.Location = new System.Drawing.Point(304, 8);
            this.btnSiguiente.Name = "btnSiguiente";
            this.btnSiguiente.Size = new System.Drawing.Size(30, 21);
            this.btnSiguiente.TabIndex = 7;
            this.btnSiguiente.Text = ">";
            this.btnSiguiente.UseVisualStyleBackColor = false;
            this.btnSiguiente.Click += new System.EventHandler(this.btnSiguiente_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(81, 11);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(60, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Boleto nº";
            // 
            // totalBoletos
            // 
            this.totalBoletos.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.totalBoletos.Location = new System.Drawing.Point(246, 10);
            this.totalBoletos.Name = "totalBoletos";
            this.totalBoletos.Size = new System.Drawing.Size(50, 13);
            this.totalBoletos.TabIndex = 3;
            this.totalBoletos.Text = "1";
            this.totalBoletos.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(222, 10);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(21, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "de";
            // 
            // boletoActual
            // 
            this.boletoActual.BackColor = System.Drawing.Color.WhiteSmoke;
            this.boletoActual.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.boletoActual.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.boletoActual.Location = new System.Drawing.Point(142, 8);
            this.boletoActual.Name = "boletoActual";
            this.boletoActual.Size = new System.Drawing.Size(50, 21);
            this.boletoActual.TabIndex = 25;
            this.boletoActual.Text = "1";
            this.boletoActual.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // controlBoleto1
            // 
            this.controlBoleto1.BackColor = System.Drawing.Color.Bisque;
            this.controlBoleto1.Location = new System.Drawing.Point(11, 40);
            this.controlBoleto1.Name = "controlBoleto1";
            this.controlBoleto1.Size = new System.Drawing.Size(349, 282);
            this.controlBoleto1.TabIndex = 0;
            // 
            // BoletoFrm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.Color.Bisque;
            this.ClientSize = new System.Drawing.Size(370, 332);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.controlBoleto1);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(386, 368);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(386, 368);
            this.Name = "BoletoFrm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Load += new System.EventHandler(this.BoletoFrm_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

		}
		#endregion

		private void BoletoFrm_Load(object sender, EventArgs e)
		{
			Cursor=Cursors.WaitCursor;
            while (ArchivoCombinacion.Length == 0)
			{
				Application.DoEvents();
			}
            string archivoEntrada = ArchivoCombinacion;
			Text = Path.GetFileName(archivoEntrada);
            IArchivoColumnas archComb = new ArchivoColumnasTexto(archivoEntrada);						
			// Calcula las apuestas que contiene el fichero
			controlBoleto1.CreaMatriz(Convert.ToInt32(archComb.ObtenNumCols()));
			boletos=controlBoleto1.boletos ;
			totalBoletos.Text=boletos.ToString();
			Application.DoEvents();
			for(int i=0;i<controlBoleto1.apuestas;i++) 
			{
				if(archComb.SiguienteColumna()) 
				{
					controlBoleto1.columna[i]= archComb.LeeColumnaSinComas();
				}
			}
			archComb.Cerrar();
			controlBoleto1.OrdenarMatrizColumnas(ordenarPor,tipoOrden);
			LlenarBoleto(0);
			Cursor=Cursors.Default;
		}

		private void btnPrimero_Click(object sender, EventArgs e)
		{
			LlenarBoleto(0);
		}

		private void btnAnterior_Click(object sender, EventArgs e)
		{
		    int tmp = Convert.ToInt32(boletoActual.Text)-2;
		    LlenarBoleto(tmp);
		}

	    private void btnSiguiente_Click(object sender, EventArgs e)
	    {
	        LlenarBoleto(Convert.ToInt32(boletoActual.Text));
	    }

	    private void btnUltimo_Click(object sender, EventArgs e)
		{
			LlenarBoleto(boletos-1);
		}

		private void btnIr_Click(object sender, EventArgs e)
		{
		    int tmp = Convert.ToInt16(boletoActual.Text);
		    if (tmp<1 || tmp>boletos)
			{
				MessageBox.Show("El número de boleto está fuera de rango");
				return;
			}
		    LlenarBoleto(tmp-1);
		}

	    private void LlenarBoleto(int numBol)
		{
			controlBoleto1.LlenarBoleto(numBol);
			btnPrimero.Visible=true;
			btnAnterior.Visible=true;
			btnUltimo.Visible=true;
			btnSiguiente.Visible=true;
			btnIr.Visible=true;
			if(numBol==0)
			{
				btnPrimero.Visible=false;
				btnAnterior.Visible=false;
			}
			if(numBol==boletos-1)
			{
				btnUltimo.Visible=false;
				btnSiguiente.Visible=false;
			}
			if(boletos==1)
			{
				btnPrimero.Visible=false;
				btnAnterior.Visible=false;
				btnUltimo.Visible=false;
				btnSiguiente.Visible=false;
				btnIr.Visible=false;
			}
			numBol++;
			boletoActual.Text=numBol.ToString();
		}

	}
}
