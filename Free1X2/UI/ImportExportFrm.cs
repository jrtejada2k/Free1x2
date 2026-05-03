using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using Free1X2.EntradaSalida;

using Free1X2.UI.Modern.Theming;
namespace Free1X2.UI
{
	/// <summary>
	/// Descripción breve de ImportExportFrm.
	/// </summary>
	public class ImportExportFrm : System.Windows.Forms.Form
	{
		private System.Windows.Forms.TextBox txFicheroEntrada;
		private System.Windows.Forms.Label lblCombinacion;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnAbrirEntrada;
		private System.Windows.Forms.Button btnAbrirSalida;
		private System.Windows.Forms.TextBox txtFicheroSalida;
		private int conversion=0;
		private string[] columnas;
		private OpenFileDialog abreFiltroDialog = new OpenFileDialog();
		private System.Windows.Forms.Label lblColsEntrada;
		private System.Windows.Forms.Label lblColsSalida;
        private RadioButton rConv3;
        private RadioButton rConv4;

		/// <summary>
		/// Variable del diseñador requerida.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public ImportExportFrm()
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

		#region Código generado por el Diseñador de Windows Forms
		/// <summary>
		/// Método necesario para admitir el Diseñador. No se puede modificar
		/// el contenido del método con el editor de código.
		/// </summary>
		private void InitializeComponent()
		{
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ImportExportFrm));
            this.btnAbrirEntrada = new System.Windows.Forms.Button();
            this.txFicheroEntrada = new System.Windows.Forms.TextBox();
            this.lblCombinacion = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rConv4 = new System.Windows.Forms.RadioButton();
            this.rConv3 = new System.Windows.Forms.RadioButton();
            this.btnAbrirSalida = new System.Windows.Forms.Button();
            this.txtFicheroSalida = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOk = new System.Windows.Forms.Button();
            this.lblColsEntrada = new System.Windows.Forms.Label();
            this.lblColsSalida = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnAbrirEntrada
            // 
            this.btnAbrirEntrada.BackColor = System.Drawing.Color.LightSalmon;
            this.btnAbrirEntrada.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnAbrirEntrada.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAbrirEntrada.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnAbrirEntrada.Image = ((System.Drawing.Image)(resources.GetObject("btnAbrirEntrada.Image")));
            this.btnAbrirEntrada.Location = new System.Drawing.Point(165, 105);
            this.btnAbrirEntrada.Name = "btnAbrirEntrada";
            this.btnAbrirEntrada.Size = new System.Drawing.Size(24, 21);
            this.btnAbrirEntrada.TabIndex = 712;
            this.btnAbrirEntrada.UseVisualStyleBackColor = false;
            this.btnAbrirEntrada.Click += new System.EventHandler(this.btnAbrirEntrada_Click);
            // 
            // txFicheroEntrada
            // 
            this.txFicheroEntrada.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txFicheroEntrada.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txFicheroEntrada.Location = new System.Drawing.Point(190, 105);
            this.txFicheroEntrada.Name = "txFicheroEntrada";
            this.txFicheroEntrada.Size = new System.Drawing.Size(249, 21);
            this.txFicheroEntrada.TabIndex = 711;
            // 
            // lblCombinacion
            // 
            this.lblCombinacion.BackColor = System.Drawing.Color.Transparent;
            this.lblCombinacion.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCombinacion.Location = new System.Drawing.Point(30, 105);
            this.lblCombinacion.Name = "lblCombinacion";
            this.lblCombinacion.Size = new System.Drawing.Size(129, 21);
            this.lblCombinacion.TabIndex = 710;
            this.lblCombinacion.Text = "Fichero de entrada";
            this.lblCombinacion.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rConv4);
            this.groupBox1.Controls.Add(this.rConv3);
            this.groupBox1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.ForeColor = System.Drawing.Color.Maroon;
            this.groupBox1.Location = new System.Drawing.Point(10, 16);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(448, 76);
            this.groupBox1.TabIndex = 714;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Tipo de conversión";
            // 
            // rConv4
            // 
            this.rConv4.ForeColor = System.Drawing.Color.Black;
            this.rConv4.Location = new System.Drawing.Point(20, 45);
            this.rConv4.Name = "rConv4";
            this.rConv4.Size = new System.Drawing.Size(409, 22);
            this.rConv4.TabIndex = 716;
            this.rConv4.Text = "TXT (*.txt) a Separadas por comas (*.csv)";
            this.rConv4.CheckedChanged += new System.EventHandler(this.cambiarConversion);
            // 
            // rConv3
            // 
            this.rConv3.Checked = true;
            this.rConv3.ForeColor = System.Drawing.Color.Black;
            this.rConv3.Location = new System.Drawing.Point(20, 21);
            this.rConv3.Name = "rConv3";
            this.rConv3.Size = new System.Drawing.Size(409, 22);
            this.rConv3.TabIndex = 715;
            this.rConv3.TabStop = true;
            this.rConv3.Text = "Separadas por comas (*.csv) a TXT (*.txt)";
            this.rConv3.CheckedChanged += new System.EventHandler(this.cambiarConversion);
            // 
            // btnAbrirSalida
            // 
            this.btnAbrirSalida.BackColor = System.Drawing.Color.LightSalmon;
            this.btnAbrirSalida.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnAbrirSalida.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAbrirSalida.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnAbrirSalida.Image = ((System.Drawing.Image)(resources.GetObject("btnAbrirSalida.Image")));
            this.btnAbrirSalida.Location = new System.Drawing.Point(165, 131);
            this.btnAbrirSalida.Name = "btnAbrirSalida";
            this.btnAbrirSalida.Size = new System.Drawing.Size(24, 21);
            this.btnAbrirSalida.TabIndex = 717;
            this.btnAbrirSalida.UseVisualStyleBackColor = false;
            this.btnAbrirSalida.Click += new System.EventHandler(this.btnAbrirSalida_Click);
            // 
            // txtFicheroSalida
            // 
            this.txtFicheroSalida.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtFicheroSalida.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtFicheroSalida.Location = new System.Drawing.Point(190, 131);
            this.txtFicheroSalida.Name = "txtFicheroSalida";
            this.txtFicheroSalida.Size = new System.Drawing.Size(249, 21);
            this.txtFicheroSalida.TabIndex = 716;
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(30, 131);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(129, 21);
            this.label1.TabIndex = 715;
            this.label1.Text = "Fichero de salida";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.Color.DarkSalmon;
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnCancel.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnCancel.Image = ((System.Drawing.Image)(resources.GetObject("btnCancel.Image")));
            this.btnCancel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCancel.Location = new System.Drawing.Point(242, 169);
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
            this.btnOk.BackColor = System.Drawing.Color.DarkSalmon;
            this.btnOk.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnOk.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnOk.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnOk.Image = ((System.Drawing.Image)(resources.GetObject("btnOk.Image")));
            this.btnOk.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnOk.Location = new System.Drawing.Point(127, 169);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(100, 32);
            this.btnOk.TabIndex = 719;
            this.btnOk.Text = "&Hacer";
            this.btnOk.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnOk.UseVisualStyleBackColor = false;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // lblColsEntrada
            // 
            this.lblColsEntrada.AutoSize = true;
            this.lblColsEntrada.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblColsEntrada.Location = new System.Drawing.Point(427, 176);
            this.lblColsEntrada.Name = "lblColsEntrada";
            this.lblColsEntrada.Size = new System.Drawing.Size(0, 13);
            this.lblColsEntrada.TabIndex = 720;
            // 
            // lblColsSalida
            // 
            this.lblColsSalida.AutoSize = true;
            this.lblColsSalida.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblColsSalida.Location = new System.Drawing.Point(427, 204);
            this.lblColsSalida.Name = "lblColsSalida";
            this.lblColsSalida.Size = new System.Drawing.Size(0, 13);
            this.lblColsSalida.TabIndex = 721;
            this.lblColsSalida.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ImportExportFrm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.Color.Bisque;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(468, 215);
            this.Controls.Add(this.lblColsSalida);
            this.Controls.Add(this.lblColsEntrada);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnAbrirSalida);
            this.Controls.Add(this.txtFicheroSalida);
            this.Controls.Add(this.txFicheroEntrada);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnAbrirEntrada);
            this.Controls.Add(this.lblCombinacion);
            this.MaximizeBox = false;
            this.Name = "ImportExportFrm";
            this.Text = "Importar / Exportar columnas";
            this.Load += new System.EventHandler(this.ImportExportFrm_Load);
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion



		private void ImportExportFrm_Load(object sender, System.EventArgs e)
		{
			abreFiltroDialog.InitialDirectory = Application.StartupPath+"/Columnas/" ;
		}

		private void cambiarConversion(object sender, System.EventArgs e)
		{
            if (rConv3.Checked)
            {
                conversion = 2;
            }
            else
            {
                conversion = 3;
            }
		}

		private void btnAbrirEntrada_Click(object sender, System.EventArgs e)
		{
            if (conversion == 2)
            {
                abreFiltroDialog.Filter = "Separadas por comas(*.csv)|*.csv|Todos los ficheros (*.*)|*.*";
            }
            else
            {
                abreFiltroDialog.Filter = "Columnas(*.txt)|*.txt|Todos los ficheros (*.*)|*.*";
            }
            if (abreFiltroDialog.ShowDialog() == DialogResult.OK)
            {
                txFicheroEntrada.Text = abreFiltroDialog.FileName;

                if (conversion == 2)
                {
                    IArchivoColumnas cols = new ArchivoColumnasTexto(abreFiltroDialog.FileName);
                    try
                    {
                        columnas = cols.LeerTodasColsSeparadasPorComas();
                        txtFicheroSalida.Text = txFicheroEntrada.Text.ToLower().Replace(".csv", ".txt");
                        lblColsEntrada.Text = columnas.Length.ToString() + " columnas.";
                    }
                    catch
                    {
                        lblColsEntrada.Text = "";
                        MessageBox.Show("No se ha podido leer el fichero de entrada", "Free1X2", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    cols.Cerrar();
                }
                else
                {
                    IArchivoColumnas cols = new ArchivoColumnasTexto(abreFiltroDialog.FileName);
                    try
                    {
                        columnas = cols.LeerTodasCols(true);
                        txtFicheroSalida.Text = txFicheroEntrada.Text.ToLower().Replace(".txt", ".csv");
                        lblColsEntrada.Text = columnas.Length.ToString() + " columnas.";
                    }
                    catch
                    {
                        lblColsEntrada.Text = "";
                        MessageBox.Show("No se ha podido leer el fichero de entrada", "Free1X2", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    cols.Cerrar();
                }
            }
		}

        private void btnAbrirSalida_Click(object sender, System.EventArgs e)
        {
            if (conversion == 0)
                abreFiltroDialog.Filter = "Columnas(*.txt)|*.txt|Todos los ficheros (*.*)|*.*";
            else if (conversion == 1)
            {
                abreFiltroDialog.Filter = "Columnas(*.cols)|*.cols|Todos los ficheros (*.*)|*.*";
            }
            else if (conversion == 2)
            {
                abreFiltroDialog.Filter = "Columnas(*.txt)|*.txt|Todos los ficheros (*.*)|*.*";
            }
            else
            {
                abreFiltroDialog.Filter = "Columnas(*.csv)|*.csv|Todos los ficheros (*.*)|*.*";
            }
            if (abreFiltroDialog.ShowDialog() == DialogResult.OK)
            {
                txFicheroEntrada.Text = abreFiltroDialog.FileName;
            }
        }

		private void btnCancel_Click(object sender, System.EventArgs e)
		{
			this.Close();
		}

		private void btnOk_Click(object sender, System.EventArgs e)
		{
			if(txtFicheroSalida.Text.Length==0) return;
			if(columnas==null || columnas.Length==0) MessageBox.Show("No se ha cargado el fichero de entrada o no tiene columnas.","Free1X2",MessageBoxButtons.OK,MessageBoxIcon.Error);

            if (txFicheroEntrada.Text != txtFicheroSalida.Text)
            {
                lblColsSalida.Text = "";
                FileInfo info = new FileInfo(txtFicheroSalida.Text);
                if (info.Exists)
                {
                    if (MessageBox.Show("Ya existe el fichero de salida. ¿Sobreescribir?", "Free1X2", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.Cancel)
                        return;
                }
                switch (conversion)
                {
                    case 2:
                        {
                            IArchivoColumnas cols = new ArchivoColumnasTexto(txtFicheroSalida.Text);
                            cols.GuardarTodasCols(columnas, false);
                            cols.Cerrar();
                            lblColsSalida.Text = cols.ObtenNumCols().ToString() + " columnas.";
                        }
                        break;
                    default:
                        {
                            IArchivoColumnas cols = new ArchivoColumnasTexto(txtFicheroSalida.Text);
                            cols.GuardarTodasCols(columnas, true);
                            cols.Cerrar();
                            lblColsSalida.Text = cols.ObtenNumCols().ToString() + " columnas.";
                        }
                        break;
                }
                MessageBox.Show("Transformación finalizada", "Importar/Exportar");
            }
            else
            {
                MessageBox.Show("Los archivos de entrada y salida deben ser distintos", "Error", MessageBoxButtons.OK);
                return;
            }
		}


	}
}
