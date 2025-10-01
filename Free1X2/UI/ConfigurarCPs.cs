using System;
using System.Data;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using Free1X2.EntradaSalida;

namespace Free1X2.EntradaSalida.GenerarCPs
{
	/// <summary>
	/// Descripción breve de ConfigCPsFrm.
	/// </summary>
	public class ConfigCPsFrm : System.Windows.Forms.Form
	{
		protected ColumnasProbables dsConfCol;
		protected int iTipo;
		private System.Windows.Forms.Button button3;
		private System.Windows.Forms.Button button5;
		private System.Windows.Forms.DataGrid dgTipos;
		private System.Windows.Forms.DataGridTableStyle dgT;
		private System.Windows.Forms.DataGridTextBoxColumn dTipo;
		private System.Windows.Forms.DataGridTextBoxColumn dNombre;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.DataGridTableStyle dgC;
		private System.Windows.Forms.DataGridTextBoxColumn csDesde;
		private System.Windows.Forms.DataGridTextBoxColumn csHasta;
		private System.Windows.Forms.DataGridBoolColumn csFFijos;
		private System.Windows.Forms.DataGridTextBoxColumn csNFijos;
		private System.Windows.Forms.DataGridBoolColumn csFDobles;
		private System.Windows.Forms.DataGridTextBoxColumn csNDobles;
		private System.Windows.Forms.DataGridBoolColumn csFTriples;
		private System.ComponentModel.IContainer components=null;

		public ConfigCPsFrm()
		{
			InitializeComponent();
			//cargamos los datos en memoria:
			DatosHelper dh = new DatosHelper();
			dsConfCol = dh.ObtenerDatos();
            Free1X2.UI.FormulariosHelper fH = new Free1X2.UI.FormulariosHelper();
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConfigCPsFrm));
            this.button3 = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.dgTipos = new System.Windows.Forms.DataGrid();
            this.dgT = new System.Windows.Forms.DataGridTableStyle();
            this.dTipo = new System.Windows.Forms.DataGridTextBoxColumn();
            this.dNombre = new System.Windows.Forms.DataGridTextBoxColumn();
            this.dgC = new System.Windows.Forms.DataGridTableStyle();
            this.csDesde = new System.Windows.Forms.DataGridTextBoxColumn();
            this.csHasta = new System.Windows.Forms.DataGridTextBoxColumn();
            this.csFFijos = new System.Windows.Forms.DataGridBoolColumn();
            this.csNFijos = new System.Windows.Forms.DataGridTextBoxColumn();
            this.csFDobles = new System.Windows.Forms.DataGridBoolColumn();
            this.csNDobles = new System.Windows.Forms.DataGridTextBoxColumn();
            this.csFTriples = new System.Windows.Forms.DataGridBoolColumn();
            this.button1 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgTipos)).BeginInit();
            this.SuspendLayout();
            // 
            // button3
            // 
            this.button3.BackColor = System.Drawing.Color.DarkSalmon;
            this.button3.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.button3.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button3.Image = ((System.Drawing.Image)(resources.GetObject("button3.Image")));
            this.button3.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button3.Location = new System.Drawing.Point(88, 272);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(128, 32);
            this.button3.TabIndex = 5;
            this.button3.Text = "Guardar";
            this.button3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.button3.UseVisualStyleBackColor = false;
            this.button3.Visible = false;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button5
            // 
            this.button5.BackColor = System.Drawing.Color.MistyRose;
            this.button5.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button5.Location = new System.Drawing.Point(400, 264);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(56, 16);
            this.button5.TabIndex = 22;
            this.button5.TabStop = false;
            this.button5.Text = "leyenda";
            this.button5.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.button5.UseVisualStyleBackColor = false;
            this.button5.Visible = false;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // dgTipos
            // 
            this.dgTipos.AllowSorting = false;
            this.dgTipos.BackgroundColor = System.Drawing.SystemColors.InactiveBorder;
            this.dgTipos.CaptionText = "Tipos de columnas";
            this.dgTipos.DataMember = "";
            this.dgTipos.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.dgTipos.Location = new System.Drawing.Point(24, 24);
            this.dgTipos.Name = "dgTipos";
            this.dgTipos.RowHeaderWidth = 5;
            this.dgTipos.Size = new System.Drawing.Size(432, 240);
            this.dgTipos.TabIndex = 23;
            this.dgTipos.TableStyles.AddRange(new System.Windows.Forms.DataGridTableStyle[] {
            this.dgT,
            this.dgC});
            this.dgTipos.TabStop = false;
            this.dgTipos.DataSourceChanged += new System.EventHandler(this.dgTipos_DataSourceChanged);
            this.dgTipos.CurrentCellChanged += new System.EventHandler(this.dgTipos_CurrentCellChanged);
            // 
            // dgT
            // 
            this.dgT.AllowSorting = false;
            this.dgT.DataGrid = this.dgTipos;
            this.dgT.GridColumnStyles.AddRange(new System.Windows.Forms.DataGridColumnStyle[] {
            this.dTipo,
            this.dNombre});
            this.dgT.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.dgT.MappingName = "Tipos de CPs";
            // 
            // dTipo
            // 
            this.dTipo.Alignment = System.Windows.Forms.HorizontalAlignment.Center;
            this.dTipo.Format = "";
            this.dTipo.FormatInfo = null;
            this.dTipo.HeaderText = "#";
            this.dTipo.MappingName = "Tipo";
            this.dTipo.NullText = "";
            this.dTipo.Width = 30;
            // 
            // dNombre
            // 
            this.dNombre.Format = "";
            this.dNombre.FormatInfo = null;
            this.dNombre.MappingName = "Nombre";
            this.dNombre.NullText = "";
            this.dNombre.Width = 150;
            // 
            // dgC
            // 
            this.dgC.DataGrid = this.dgTipos;
            this.dgC.GridColumnStyles.AddRange(new System.Windows.Forms.DataGridColumnStyle[] {
            this.csDesde,
            this.csHasta,
            this.csFFijos,
            this.csNFijos,
            this.csFDobles,
            this.csNDobles,
            this.csFTriples});
            this.dgC.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.dgC.MappingName = "Configuracion de CPs";
            // 
            // csDesde
            // 
            this.csDesde.Format = "";
            this.csDesde.FormatInfo = null;
            this.csDesde.HeaderText = "Desde";
            this.csDesde.MappingName = "desde";
            this.csDesde.NullText = "";
            this.csDesde.Width = 40;
            // 
            // csHasta
            // 
            this.csHasta.Format = "";
            this.csHasta.FormatInfo = null;
            this.csHasta.HeaderText = "Hasta";
            this.csHasta.MappingName = "Hasta";
            this.csHasta.NullText = "";
            this.csHasta.Width = 40;
            // 
            // csFFijos
            // 
            this.csFFijos.Alignment = System.Windows.Forms.HorizontalAlignment.Center;
            this.csFFijos.HeaderText = "Forzar Fijos";
            this.csFFijos.MappingName = "Forzar Fijos";
            this.csFFijos.NullText = "0";
            this.csFFijos.NullValue = "False";
            this.csFFijos.Width = 75;
            // 
            // csNFijos
            // 
            this.csNFijos.Format = "";
            this.csNFijos.FormatInfo = null;
            this.csNFijos.HeaderText = "Nº Fijos";
            this.csNFijos.MappingName = "Num Fijos";
            this.csNFijos.NullText = "";
            this.csNFijos.Width = 50;
            // 
            // csFDobles
            // 
            this.csFDobles.Alignment = System.Windows.Forms.HorizontalAlignment.Center;
            this.csFDobles.HeaderText = "Forzar Dobles";
            this.csFDobles.MappingName = "Forzar Dobles";
            this.csFDobles.NullText = "0";
            this.csFDobles.NullValue = "False";
            this.csFDobles.Width = 75;
            // 
            // csNDobles
            // 
            this.csNDobles.Format = "";
            this.csNDobles.FormatInfo = null;
            this.csNDobles.HeaderText = "Nº Dobles";
            this.csNDobles.MappingName = "Num Dobles";
            this.csNDobles.NullText = "";
            this.csNDobles.Width = 55;
            // 
            // csFTriples
            // 
            this.csFTriples.Alignment = System.Windows.Forms.HorizontalAlignment.Center;
            this.csFTriples.HeaderText = "Forzar Triples";
            this.csFTriples.MappingName = "Forzar Triples";
            this.csFTriples.NullText = "0";
            this.csFTriples.NullValue = "False";
            this.csFTriples.Width = 75;
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.Color.DarkSalmon;
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.button1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.Image = ((System.Drawing.Image)(resources.GetObject("button1.Image")));
            this.button1.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button1.Location = new System.Drawing.Point(232, 272);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(128, 32);
            this.button1.TabIndex = 24;
            this.button1.Text = "Volver";
            this.button1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Visible = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // ConfigCPsFrm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.Color.Bisque;
            this.ClientSize = new System.Drawing.Size(480, 318);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.dgTipos);
            this.Controls.Add(this.button5);
            this.Controls.Add(this.button3);
            this.Name = "ConfigCPsFrm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Configurar Columnas Probables";
            this.Load += new System.EventHandler(this.ConfigCPsFrm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgTipos)).EndInit();
            this.ResumeLayout(false);

		}
		#endregion

		private void ConfigCPsFrm_Load(object sender, System.EventArgs e)
		{
			dgTipos.SetDataBinding(dsConfCol,"Tipos de CPs");
		}

		private void button3_Click(object sender, System.EventArgs e)
		{
			//guardamos datos a disco
			DatosHelper datosHelper = new DatosHelper();
			datosHelper.GuardarDatos(dsConfCol );
			button3.Visible=false;
		}

		private void button5_Click(object sender, System.EventArgs e)
		{
			string tab=((char) 9).ToString();
			string nl="\r\n";
			string msg;
			msg ="#             " + tab + "Número de índice de las columnas." + nl;
			msg+="Nombre        " + tab + "Nombre descriptivo de las columnas." + nl + nl;
			msg+="Desde         " + tab + "Mínima puntuación para incluir el signo." + nl;
			msg+="Hasta         " + tab + "Máxima puntuación para incluir el signo." + nl;
			msg+="Forzar Fijos  " + tab + "Sólo columnas de fijos (los más fijos)" + nl;
			msg+="Nº Fijos      " + tab + "Nº de fijos en columna de fijos" + nl;
			msg+="Forzar Dobles " + tab + "Sólo columnas de dobles (los más dobles)" + nl;
			msg+="Nº Dobles     " + tab + "Nº de dobles en columna de dobles" + nl;
			msg+="Forzar Triples" + tab + "Fuerza el triple si ningún signo se incluye en rango" + nl + nl;
			msg+="Si se hacen columnas de sólo fijos o dobles, se recomienda poner Desde=0 y Hasta=100." + nl;
			MessageBox.Show(msg,"Leyenda",MessageBoxButtons.OK,MessageBoxIcon.Information);
		}


		private void dgTipos_DataSourceChanged(object sender, System.EventArgs e)
		{
			button3.Visible=false;
		}

		private void dgTipos_CurrentCellChanged(object sender, System.EventArgs e)
		{
			button3.Visible=true;
		}

		private void button1_Click(object sender, System.EventArgs e)
		{
			this.Close();
		}
	}
}
