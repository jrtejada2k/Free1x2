using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Free1X2.EntradaSalida.GenerarCPs;
using Free1X2.Utils;
using Free1X2.UI.Controls;

namespace Free1X2.UI
{
	/// <summary>
	/// Descripción breve de GenerarCPs.
	/// </summary>
	public class GenerarCPs : Form
	{
		private Button btnConfigurar;
		private Button btnImportarVal;
		private Button btnCancel;
		private Button btnOK;
		protected Valoracion[] valores;
		private Label label1;
        private NumTextBox numJornada;
		private Button btnSeparador;
        private Button btnDiferencias;
        private Panel panelPartidos;
        private Label label2;
		/// <summary>
		/// Variable del diseñador requerida.
		/// </summary>
		private System.ComponentModel.Container components;

		public GenerarCPs()
		{
			InitializeComponent();
            FormulariosHelper fH = new FormulariosHelper();
            fH.Traducir(this);
            LlenarControles();
		}
        private void LlenarControles()
        {
            int x = 0;
            int y = 0;
            for (int i = 1; i <= VariablesGlobales.NumeroPartidos; i++)
            {
                GeneradorOptions gO = new GeneradorOptions();
                gO.NumeroPartido = i;
                if (i < 10)
                {
                    gO.Name = "val_0" + i;
                }
                else
                {
                    gO.Name = "val_" + i;
                }
                gO.Location = new Point(x, y);
                panelPartidos.Controls.Add(gO);
                y += gO.Size.Height + 1;
            }
        }
        /// <summary>
        /// Limpiar los recursos que se estén utilizando.
        /// </summary>
        protected override void Dispose(bool disposing)
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GenerarCPs));
            this.btnConfigurar = new System.Windows.Forms.Button();
            this.btnImportarVal = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.btnSeparador = new System.Windows.Forms.Button();
            this.btnDiferencias = new System.Windows.Forms.Button();
            this.panelPartidos = new System.Windows.Forms.Panel();
            this.numJornada = new Free1X2.UI.Controls.NumTextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnConfigurar
            // 
            this.btnConfigurar.BackColor = System.Drawing.Color.DarkSalmon;
            this.btnConfigurar.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnConfigurar.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnConfigurar.Image = ((System.Drawing.Image)(resources.GetObject("btnConfigurar.Image")));
            this.btnConfigurar.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnConfigurar.Location = new System.Drawing.Point(208, 34);
            this.btnConfigurar.Name = "btnConfigurar";
            this.btnConfigurar.Size = new System.Drawing.Size(161, 32);
            this.btnConfigurar.TabIndex = 0;
            this.btnConfigurar.Text = "Configurar Columnas";
            this.btnConfigurar.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnConfigurar.UseVisualStyleBackColor = false;
            this.btnConfigurar.Click += new System.EventHandler(this.btnConfigurar_Click);
            // 
            // btnImportarVal
            // 
            this.btnImportarVal.BackColor = System.Drawing.Color.DarkSalmon;
            this.btnImportarVal.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnImportarVal.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnImportarVal.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnImportarVal.Image = ((System.Drawing.Image)(resources.GetObject("btnImportarVal.Image")));
            this.btnImportarVal.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnImportarVal.Location = new System.Drawing.Point(208, 67);
            this.btnImportarVal.Name = "btnImportarVal";
            this.btnImportarVal.Size = new System.Drawing.Size(161, 32);
            this.btnImportarVal.TabIndex = 63;
            this.btnImportarVal.Text = "Importar %";
            this.btnImportarVal.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnImportarVal.UseVisualStyleBackColor = false;
            this.btnImportarVal.Click += new System.EventHandler(this.btnImportarVal_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.Color.DarkSalmon;
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnCancel.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnCancel.Image = ((System.Drawing.Image)(resources.GetObject("btnCancel.Image")));
            this.btnCancel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCancel.Location = new System.Drawing.Point(289, 193);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(80, 32);
            this.btnCancel.TabIndex = 65;
            this.btnCancel.Text = "Cancelar";
            this.btnCancel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOK
            // 
            this.btnOK.BackColor = System.Drawing.Color.DarkSalmon;
            this.btnOK.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnOK.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnOK.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnOK.Image = ((System.Drawing.Image)(resources.GetObject("btnOK.Image")));
            this.btnOK.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnOK.Location = new System.Drawing.Point(208, 193);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(80, 32);
            this.btnOK.TabIndex = 64;
            this.btnOK.Text = "OK";
            this.btnOK.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnOK.UseVisualStyleBackColor = false;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(208, 148);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(123, 21);
            this.label1.TabIndex = 80;
            this.label1.Text = "Jornada:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnSeparador
            // 
            this.btnSeparador.BackColor = System.Drawing.Color.DarkSalmon;
            this.btnSeparador.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnSeparador.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSeparador.Image = ((System.Drawing.Image)(resources.GetObject("btnSeparador.Image")));
            this.btnSeparador.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSeparador.Location = new System.Drawing.Point(208, 264);
            this.btnSeparador.Name = "btnSeparador";
            this.btnSeparador.Size = new System.Drawing.Size(161, 44);
            this.btnSeparador.TabIndex = 86;
            this.btnSeparador.Text = "Separador de \r\nporcentajes";
            this.btnSeparador.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnSeparador.UseVisualStyleBackColor = false;
            this.btnSeparador.Click += new System.EventHandler(this.btnSeparador_Click);
            // 
            // btnDiferencias
            // 
            this.btnDiferencias.BackColor = System.Drawing.Color.DarkSalmon;
            this.btnDiferencias.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnDiferencias.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDiferencias.Image = ((System.Drawing.Image)(resources.GetObject("btnDiferencias.Image")));
            this.btnDiferencias.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnDiferencias.Location = new System.Drawing.Point(208, 309);
            this.btnDiferencias.Name = "btnDiferencias";
            this.btnDiferencias.Size = new System.Drawing.Size(161, 44);
            this.btnDiferencias.TabIndex = 87;
            this.btnDiferencias.Text = "CPs por diferencias";
            this.btnDiferencias.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnDiferencias.UseVisualStyleBackColor = false;
            this.btnDiferencias.Click += new System.EventHandler(this.btnDiferencias_Click);
            // 
            // panelPartidos
            // 
            this.panelPartidos.Location = new System.Drawing.Point(5, 22);
            this.panelPartidos.Name = "panelPartidos";
            this.panelPartidos.Size = new System.Drawing.Size(200, 345);
            this.panelPartidos.TabIndex = 93;
            // 
            // numJornada
            // 
            this.numJornada.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.numJornada.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.numJornada.Location = new System.Drawing.Point(337, 148);
            this.numJornada.Name = "numJornada";
            this.numJornada.Size = new System.Drawing.Size(32, 21);
            this.numJornada.TabIndex = 81;
            this.numJornada.Text = "0";
            this.numJornada.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.SaddleBrown;
            this.label2.Location = new System.Drawing.Point(5, 1);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(200, 21);
            this.label2.TabIndex = 94;
            this.label2.Text = "Valoración";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // GenerarCPs
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.Color.Bisque;
            this.ClientSize = new System.Drawing.Size(381, 381);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.panelPartidos);
            this.Controls.Add(this.btnDiferencias);
            this.Controls.Add(this.btnSeparador);
            this.Controls.Add(this.numJornada);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnImportarVal);
            this.Controls.Add(this.btnConfigurar);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "GenerarCPs";
            this.Text = "Generador de Columnas Probables";
            this.Load += new System.EventHandler(this.GenerarCPs_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		private void btnCancel_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void btnImportarVal_Click(object sender, EventArgs e)
		{
			valores = new Valoracion[VariablesGlobales.NumeroPartidos];

		    OpenFileDialog abreValIn = new OpenFileDialog();
			abreValIn.InitialDirectory = "Condiciones\\" ;
			abreValIn.Filter = "A.Valoracion(*.txt)|*.txt|Todos los archivos(*.*)|*.*";
            if (abreValIn.ShowDialog() == DialogResult.OK)
            {
                Porcentajes Pct = new Porcentajes(abreValIn.FileName);

                for (int i = 0; i < VariablesGlobales.NumeroPartidos; i++)
                {
                    Valoracion valTmp = new Valoracion();
                    valTmp.Uno = Pct.valores[i, 0];
                    valTmp.Equis = Pct.valores[i, 1];
                    valTmp.Dos = Pct.valores[i, 2];
                    valores[i] = valTmp;
                }
                PonerValoracionPantalla(valores);
                CopiarValoracion();
            }
		}

        protected void PonerValoracionPantalla(Valoracion[] vals)
        {
            for (int i = 0; i < vals.Length; i++)
            {
                int numPartido = i + 1;
                GeneradorOptions controlVal = (GeneradorOptions)panelPartidos.Controls[i];
                if (numPartido <= VariablesGlobales.NumeroPartidos)
                {

                    controlVal.Valor_1 = vals[i].Uno.ToString();
                    controlVal.Valor_X = vals[i].Equis.ToString();
                    controlVal.Valor_2 = vals[i].Dos.ToString();
                    controlVal.Visible = true;
                }
                else
                {
                    controlVal.Visible = false;
                }
            }

        }

        private void CopiarValoracion()
        {
            valores = new Valoracion[VariablesGlobales.NumeroPartidos];

            for (int i = 0; i < panelPartidos.Controls.Count; i++)
            {
                GeneradorOptions controlVal = (GeneradorOptions)panelPartidos.Controls[i];
                Valoracion valTemp = new Valoracion();
                int numPartido = controlVal.NumeroPartido - 1;
                valTemp.Uno = Convert.ToDouble(controlVal.Valor_1);
                valTemp.Equis = Convert.ToDouble(controlVal.Valor_X);
                valTemp.Dos = Convert.ToDouble(controlVal.Valor_2);

                valores[numPartido] = valTemp;
            }
        }
			
		private void btnConfigurar_Click(object sender, EventArgs e)
		{
			ConfigCPsFrm f=new ConfigCPsFrm();
			f.ShowDialog();
		}

		private void GenerarCPs_Load(object sender, EventArgs e)
		{
			valores = new Valoracion[VariablesGlobales.NumeroPartidos];
		}

		private void btnOK_Click(object sender, EventArgs e)
		{
			int jornada = 0;
			if(numJornada.Text != "")
			{
				jornada = Convert.ToInt16(numJornada.Text);
			}

            if (jornada <= 0)
            {
                MessageBox.Show("Especifique un número de jornada","Error",MessageBoxButtons.OK);
                return; 
            }

		    // Copiamos la valoración a la matriz
			CopiarValoracion();

			// Guardar Columnas.
			// Cargamos los datos de las columnas en memoria
			DatosHelper dh = new DatosHelper();
			ColumnasProbables dsConfCol = dh.ObtenerDatos();

			IO f2;
            for (int i = 0; i < dsConfCol.Tables["Tipos de CPs"].Rows.Count; i++)
            {
                DataView dv = new DataView(dsConfCol.Tables["Configuracion de CPs"]);
                string filtro = "Tipo = " + dsConfCol.Tables["Tipos de CPs"].Rows[i]["Tipo"];
                dv.RowFilter = filtro;
                DataSet ds = LlenarDataset(dv);
                string txt = LlenarTxtColumnas(ds);
                string nombre = dsConfCol.Tables["Tipos de CPs"].Rows[i]["Nombre"].ToString();
                string fichero = Application.StartupPath + "/Condiciones/" + nombre + "_j" + jornada + ".txt";
                fichero.Replace(" ", "_");
                f2 = new IO(fichero);
                f2.GuardarTexto(txt);
                f2.Cerrar();
            }
			MessageBox.Show("Fichero(s) guardado(s) correctamente en la carpeta Condiciones","Generación de columnas",MessageBoxButtons.OK,MessageBoxIcon.Information);
		}

        private DataSet LlenarDataset(DataView dv)
        {
            CPs cps = new CPs();
            string[,] columnas = cps.CrearCPs(dv, valores);
            return LlenarDataset(columnas);
        }

		public DataSet LlenarDataset(string[,] columnas)
		{
			// Columnas
		    int i;
			
		    int longitud = columnas.Length/VariablesGlobales.NumeroPartidos;
			DataTable myDataTable = new DataTable();
		    //creamos las columnas en la tabla:
			for(i=0;i<longitud;i++)
			{
				DataColumn myDataColumn = new DataColumn();
				myDataColumn.DataType = Type.GetType("System.String");
				myDataColumn.ColumnName = i.ToString();
				// añadir a la  collecion de columnas del datatable
				myDataTable.Columns.Add(myDataColumn);
			}
			// Llenamos el dataset
			for (i = 0; i<VariablesGlobales.NumeroPartidos; i++)
			{
                
				DataRow myDataRow = myDataTable.NewRow();
				for (int j=0;j<(longitud);j++)
				{
                    string col;
					if(columnas[i,j]==null)
					{
						col=" ";
					}
					else
					{
						col=columnas[i,j];
					}
					myDataRow[j.ToString()] = col;
				}
				myDataTable.Rows.Add(myDataRow);
			}

			// crea el dataset
			DataSet myDataSet = new DataSet();
			// añade el datatable al dataset.
			myDataSet.Tables.Add(myDataTable);

			return myDataSet;
		}

		private string LlenarTxtColumnas(DataSet dsCPs)
		{
			string txt="";
			string nl="\r\n";
            for (int i = 0; i < dsCPs.Tables[0].Columns.Count; i++)
            {
                for (int j = 0; j < VariablesGlobales.NumeroPartidos; j++)
                {
                    txt += dsCPs.Tables[0].Rows[j][i].ToString();
                    if (j < VariablesGlobales.NumeroPartidos - 1)
                    {
                        txt += ",";
                    }
                }
                txt += nl;
            }
			return txt;
		}

		private void btnSeparador_Click(object sender, EventArgs e)
		{
			FiltroPorcenJB separador = new FiltroPorcenJB();
			separador.ShowDialog();
		}

		private void btnDiferencias_Click(object sender, EventArgs e)
		{
			GeneradorCPSDiferencias f=new GeneradorCPSDiferencias();
			f.ShowDialog();
		}

	}
}
