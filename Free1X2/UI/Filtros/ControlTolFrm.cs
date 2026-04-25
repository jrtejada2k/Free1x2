// created on 09/03/2004 at 22:44
// Free1X2 : Programa de quinielas "libre"
// Copyright (C) 2004 Luis Fernandez - luifer@onetel.net.uk
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
using System.Windows.Forms;
using System.Collections.Generic;
using System.Data;

using Free1X2.MotorCalculo;

using Free1X2.UI.Modern.Theming;
namespace Free1X2.UI.Filtros
{
    public class ControlTolFrm : Form
    {
        private Button btnOK;
        private Button btnCancel;
        private DataGrid dataGrid;
		
        protected ControladorTol ctrlTolerancias;
        private Label label1;
        private TextBox txtFallosControles;
        protected List<ToleranciaFiltros> arrayTolerancias;
		
        public ControlTolFrm(ControladorTol ctrlTol)
        {
            InitializeComponent();
            ctrlTolerancias = ctrlTol;
            InicializaGrid();
            InicializaDatosDG();

            txtFallosControles.Text = ctrlTolerancias.FallosPermitidos;
            FormulariosHelper fh = new FormulariosHelper();
            fh.Traducir(this);
        }
        protected override void OnLoad(System.EventArgs e)
        {
            base.OnLoad(e);
            ModernTheme.ApplyToForm(this);
        }

		
        protected void InicializaDatosDG()
        {
            arrayTolerancias = new List<ToleranciaFiltros>();
			
            ToleranciaFiltros tol; 
            ToleranciaFiltros tolGrupo; 

            //carga datos
            List<ToleranciaFiltros> ctrlGrupo = ctrlTolerancias.Tolerancias;
            int noLineas = ctrlGrupo.Count;
			
            //nos aseguramos de que hayan por lo menos 30 lineas
            if(noLineas < 30)
            {
                noLineas = 30;
            }
			
            //crea lineas en blanco
            for(int i = 0; i < noLineas; i++)
            {
                tol = new ToleranciaFiltros();
                arrayTolerancias.Add(tol);			
            }			
			
            for(int i = 0; i < ctrlGrupo.Count; i++)
            {
                tolGrupo = ctrlGrupo[i];
                tol = arrayTolerancias[i];
				
                tol.LetrasTol = tolGrupo.LetrasTol;
                tol.Aciertos = tolGrupo.Aciertos;
			
            }

            DataSet miDataset = ObtenDataSetControlFallos( arrayTolerancias );
            dataGrid.SetDataBinding(miDataset, "ControlTolerancias");
					
        }

        protected DataSet ObtenDataSetControlFallos(List<ToleranciaFiltros> arrayControlesFallo)
        {			
            DataTable myDataTable = new DataTable("ControlTolerancias");

            DataColumn myDataColumn = new DataColumn();
            myDataColumn.DataType = Type.GetType("System.String");
            myDataColumn.ColumnName = "LetrasTol";
            myDataColumn.DefaultValue = "";
            myDataTable.Columns.Add(myDataColumn);

            myDataColumn = new DataColumn();
            myDataColumn.DataType = Type.GetType("System.String");
            myDataColumn.ColumnName = "Aciertos";
            myDataColumn.DefaultValue = "";
            myDataTable.Columns.Add(myDataColumn);

            DataSet myDataSet = new DataSet();
            // Add the new DataTable to the DataSet.
            myDataSet.Tables.Add(myDataTable);

            //meter datos en el dataset
            for (int i = 0; i < arrayControlesFallo.Count; i++)
            {
                DataRow myDataRow = myDataTable.NewRow();
                myDataRow["LetrasTol"]	 = (arrayControlesFallo[i]).LetrasTol;
                myDataRow["Aciertos"] = (arrayControlesFallo[i]).Aciertos;

                myDataTable.Rows.Add(myDataRow);
            }

            return myDataSet;		
        }


        protected List<ToleranciaFiltros> ObtenArrayControlFallos(DataSet datosDS)
        {
            List<ToleranciaFiltros> arrayListDatos = new List<ToleranciaFiltros>();

            foreach(DataRow row in datosDS.Tables["ControlTolerancias"].Rows)
            {
                if(row["LetrasTol"].ToString() != "")
                {
                    ToleranciaFiltros tol = new ToleranciaFiltros();

                    tol.LetrasTol = row["LetrasTol"].ToString();
                    tol.Aciertos = row["Aciertos"].ToString();
					
                    arrayListDatos.Add(tol);
                }
            }

            return arrayListDatos;		
        }


        protected void InicializaGrid()
        {
            DataGridTableStyle tableStyle = new DataGridTableStyle();
            tableStyle.MappingName = "ControlTolerancias";
            tableStyle.ColumnHeadersVisible = true;

            //LetrasTol
            DataGridTextBoxColumn cs = new DataGridTextBoxColumn();
            cs.MappingName = "LetrasTol";
            cs.HeaderText = "Tolerancia";
            cs.Width = 100;
            tableStyle.GridColumnStyles.Add(cs);
			
            //Aciertos
            cs = new DataGridTextBoxColumn();
            cs.MappingName = "Aciertos";
            cs.HeaderText = "Aciertos";
            cs.Width = 100;
            tableStyle.GridColumnStyles.Add(cs);	
			
            dataGrid.TableStyles.Add(tableStyle);	
					
        }
        #region designer
		
		
        void InitializeComponent() 
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ControlTolFrm));
            this.dataGrid = new System.Windows.Forms.DataGrid();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.txtFallosControles = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.dataGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGrid
            // 
            this.dataGrid.BackColor = System.Drawing.Color.Bisque;
            this.dataGrid.BackgroundColor = System.Drawing.Color.Bisque;
            this.dataGrid.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dataGrid.CaptionVisible = false;
            this.dataGrid.DataMember = "";
            this.dataGrid.FlatMode = true;
            this.dataGrid.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.dataGrid.Location = new System.Drawing.Point(0, 0);
            this.dataGrid.Name = "dataGrid";
            this.dataGrid.PreferredColumnWidth = 5;
            this.dataGrid.RowHeaderWidth = 5;
            this.dataGrid.Size = new System.Drawing.Size(235, 312);
            this.dataGrid.TabIndex = 0;
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.Color.LightSalmon;
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnCancel.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancel.Image = ((System.Drawing.Image)(resources.GetObject("btnCancel.Image")));
            this.btnCancel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCancel.Location = new System.Drawing.Point(144, 320);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(80, 24);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Cancelar";
            this.btnCancel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.BtnCancelClick);
            // 
            // btnOK
            // 
            this.btnOK.BackColor = System.Drawing.Color.LightSalmon;
            this.btnOK.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnOK.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnOK.Image = ((System.Drawing.Image)(resources.GetObject("btnOK.Image")));
            this.btnOK.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnOK.Location = new System.Drawing.Point(56, 320);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(80, 24);
            this.btnOK.TabIndex = 1;
            this.btnOK.Text = "Ok";
            this.btnOK.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnOK.UseVisualStyleBackColor = false;
            this.btnOK.Click += new System.EventHandler(this.BtnOKClick);
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(248, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(128, 16);
            this.label1.TabIndex = 3;
            this.label1.Text = "Fallos Controles";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // txtFallosControles
            // 
            this.txtFallosControles.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtFallosControles.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtFallosControles.Location = new System.Drawing.Point(248, 35);
            this.txtFallosControles.Name = "txtFallosControles";
            this.txtFallosControles.Size = new System.Drawing.Size(128, 21);
            this.txtFallosControles.TabIndex = 4;
            // 
            // ControlTolFrm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.Color.Bisque;
            this.ClientSize = new System.Drawing.Size(384, 357);
            this.ControlBox = false;
            this.Controls.Add(this.txtFallosControles);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.dataGrid);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ControlTolFrm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "Control Tolerancias";
            ((System.ComponentModel.ISupportInitialize)(this.dataGrid)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion

        void BtnCancelClick(object sender, EventArgs e)
        {
            Close();
        }
		

        void BtnOKClick(object sender, EventArgs e)
        {
            //grabar tolerancias...

            DataSet miDataset = (DataSet)dataGrid.DataSource;

            arrayTolerancias = ObtenArrayControlFallos(miDataset);

            List<ToleranciaFiltros> toleranciasFinal = new List<ToleranciaFiltros>();
			
            for(int i = 0; i < arrayTolerancias.Count; i++)
            {
                ToleranciaFiltros tol = arrayTolerancias[i];

                if(tol.Aciertos != "")
                {
                    toleranciasFinal.Add( tol );
                }
            }

            ctrlTolerancias.Tolerancias = toleranciasFinal;
            ctrlTolerancias.FallosPermitidos = txtFallosControles.Text;	
						
            Close();
        }

		
    }
}