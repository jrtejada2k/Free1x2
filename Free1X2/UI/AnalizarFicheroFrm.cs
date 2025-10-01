using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using Free1X2.EntradaSalida;
using Free1X2.Analisis;
using Free1X2.MotorCalculo;

namespace Free1X2.UI
{
	/// <summary>
	/// Descripción breve de ImportExportFrm.
	/// </summary>
	public class AnalizarFicheroFrm : System.Windows.Forms.Form
	{
		private System.Windows.Forms.TextBox txFicheroEntrada;
		private System.Windows.Forms.Label lblCombinacion;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Button btnOk;
		private System.Windows.Forms.Button btnAbrirEntrada;
		//private int conversion=0;
		private string[] columnas;
		private OpenFileDialog abreFiltroDialog = new OpenFileDialog();
		private System.Windows.Forms.Label lblColsEntrada;
		private System.Windows.Forms.CheckBox chkPleno;

		/// <summary>
		/// Variable del diseñador requerida.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public AnalizarFicheroFrm()
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

		#region Código generado por el Diseñador de Windows Forms
		/// <summary>
		/// Método necesario para admitir el Diseñador. No se puede modificar
		/// el contenido del método con el editor de código.
		/// </summary>
		private void InitializeComponent()
		{
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AnalizarFicheroFrm));
            this.btnAbrirEntrada = new System.Windows.Forms.Button();
            this.txFicheroEntrada = new System.Windows.Forms.TextBox();
            this.lblCombinacion = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOk = new System.Windows.Forms.Button();
            this.lblColsEntrada = new System.Windows.Forms.Label();
            this.chkPleno = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // btnAbrirEntrada
            // 
            this.btnAbrirEntrada.BackColor = System.Drawing.Color.LightSalmon;
            this.btnAbrirEntrada.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnAbrirEntrada.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAbrirEntrada.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnAbrirEntrada.Image = ((System.Drawing.Image)(resources.GetObject("btnAbrirEntrada.Image")));
            this.btnAbrirEntrada.Location = new System.Drawing.Point(16, 40);
            this.btnAbrirEntrada.Name = "btnAbrirEntrada";
            this.btnAbrirEntrada.Size = new System.Drawing.Size(21, 21);
            this.btnAbrirEntrada.TabIndex = 712;
            this.btnAbrirEntrada.UseVisualStyleBackColor = false;
            this.btnAbrirEntrada.Click += new System.EventHandler(this.btnAbrirEntrada_Click);
            // 
            // txFicheroEntrada
            // 
            this.txFicheroEntrada.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txFicheroEntrada.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txFicheroEntrada.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txFicheroEntrada.Location = new System.Drawing.Point(38, 40);
            this.txFicheroEntrada.Name = "txFicheroEntrada";
            this.txFicheroEntrada.Size = new System.Drawing.Size(408, 21);
            this.txFicheroEntrada.TabIndex = 711;
            // 
            // lblCombinacion
            // 
            this.lblCombinacion.BackColor = System.Drawing.Color.Transparent;
            this.lblCombinacion.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCombinacion.Location = new System.Drawing.Point(16, 16);
            this.lblCombinacion.Name = "lblCombinacion";
            this.lblCombinacion.Size = new System.Drawing.Size(430, 20);
            this.lblCombinacion.TabIndex = 710;
            this.lblCombinacion.Text = "Nombre del fichero de columnas";
            this.lblCombinacion.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.BackColor = System.Drawing.Color.DarkSalmon;
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnCancel.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnCancel.Image = ((System.Drawing.Image)(resources.GetObject("btnCancel.Image")));
            this.btnCancel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCancel.Location = new System.Drawing.Point(256, 120);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(100, 32);
            this.btnCancel.TabIndex = 718;
            this.btnCancel.Text = "&Cancelar";
            this.btnCancel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOk
            // 
            this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOk.BackColor = System.Drawing.Color.DarkSalmon;
            this.btnOk.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnOk.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnOk.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnOk.Image = ((System.Drawing.Image)(resources.GetObject("btnOk.Image")));
            this.btnOk.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnOk.Location = new System.Drawing.Point(104, 120);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(100, 32);
            this.btnOk.TabIndex = 719;
            this.btnOk.Text = "&Analizar";
            this.btnOk.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnOk.UseVisualStyleBackColor = false;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // lblColsEntrada
            // 
            this.lblColsEntrada.AutoSize = true;
            this.lblColsEntrada.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblColsEntrada.Location = new System.Drawing.Point(48, 64);
            this.lblColsEntrada.Name = "lblColsEntrada";
            this.lblColsEntrada.Size = new System.Drawing.Size(0, 13);
            this.lblColsEntrada.TabIndex = 720;
            // 
            // chkPleno
            // 
            this.chkPleno.Enabled = false;
            this.chkPleno.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.chkPleno.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkPleno.Location = new System.Drawing.Point(16, 88);
            this.chkPleno.Name = "chkPleno";
            this.chkPleno.Size = new System.Drawing.Size(205, 16);
            this.chkPleno.TabIndex = 721;
            this.chkPleno.Text = "Incluir pleno al 15";
            // 
            // AnalizarFicheroFrm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.Color.Bisque;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(464, 166);
            this.ControlBox = false;
            this.Controls.Add(this.chkPleno);
            this.Controls.Add(this.lblColsEntrada);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.txFicheroEntrada);
            this.Controls.Add(this.btnAbrirEntrada);
            this.Controls.Add(this.lblCombinacion);
            this.MaximizeBox = false;
            this.Name = "AnalizarFicheroFrm";
            this.Text = "Analizar fichero de columnas";
            this.Load += new System.EventHandler(this.AnalizarFicheroFrm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion



		private void AnalizarFicheroFrm_Load(object sender, System.EventArgs e)
		{
			abreFiltroDialog.InitialDirectory = Application.StartupPath+"/Columnas/" ;
			abreFiltroDialog.Filter = "Columnas(*.txt)|*.txt|Columnas(*.cols)|*.cols|Todos los ficheros (*.*)|*.*" ;
		}

		private void btnAbrirEntrada_Click(object sender, System.EventArgs e)
		{
			if(abreFiltroDialog.ShowDialog () == DialogResult.OK)
			{
				txFicheroEntrada.Text = abreFiltroDialog.FileName;
			}
            IArchivoColumnas cols;
			try
			{
                cols  = new ArchivoColumnasTexto(txFicheroEntrada.Text);
				columnas=cols.LeerTodasCols(false);
				lblColsEntrada.Text=columnas.Length.ToString()+" columnas.";
			}
			catch
			{
				lblColsEntrada.Text="";
				MessageBox.Show("No se ha podido leer el fichero de columnas.","Free1X2",MessageBoxButtons.OK,MessageBoxIcon.Error);
				return;
			}
			cols.Cerrar();
			if(columnas.Length>0)
			{
				btnOk.Enabled=true;
				if(columnas[0].Length==15)
					chkPleno.Enabled=true;
				else
					chkPleno.Enabled=false;
			}
			else
			{
				btnOk.Enabled=false;
				chkPleno.Checked=false;
				chkPleno.Enabled=false;
			}
		}

		private void btnCancel_Click(object sender, System.EventArgs e)
		{
			this.Close();
		}

		private void btnOk_Click(object sender, System.EventArgs e)
		{
			if(txFicheroEntrada.Text.Length==0) return;
			if(columnas==null || columnas.Length==0) MessageBox.Show("No se ha cargado el fichero de entrada o no tiene columnas.","Free1X2",MessageBoxButtons.OK,MessageBoxIcon.Error);
			string fichero=txFicheroEntrada.Text;
			int pos=fichero.LastIndexOf("\\");
			if(pos>=0) fichero=fichero.Substring(pos+1);

            IArchivoColumnas aCol = new ArchivoColumnasTexto(txFicheroEntrada.Text);
            int partidos = aCol.ObtenNumSignos();
            aCol.Cerrar();

            MotorCalculo.Analizador analizadorTemp = new MotorCalculo.Analizador(partidos);
            analizadorTemp.ArchivoColumnasBase = abreFiltroDialog.FileName;
            //Inicializamos los 14triples en el analizador
            for (int i = 0; i < partidos; i++)
            {
                analizadorTemp.SetPronostico(i, "1,X,2");
            }
            analizadorTemp.AnalizaCombinacion(true,true);
		}


    }
}
