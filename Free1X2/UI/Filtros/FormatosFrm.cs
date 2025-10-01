// Free1X2 : Programa de quinielas "libre"
// Copyright (C) 2005 Luis Fernandez - luifer@onetel.net.uk
// Copyright (C) 2007 Morrison - morrison.ne@gmail.com
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
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using Free1X2.EntradaSalida;
using Free1X2.MotorCalculo;
using Free1X2.MotorCalculo.Estadisticas;

namespace Free1X2.UI.Filtros
{
    /// <summary>
    /// Summary description for FormatosFrm.
    /// </summary>
    public class FormatosFrm : Form
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components;
        private GroupBox groupBox1;
        private Button btnRepPrev;
        private Button btnRepNext;
        private Label lblNoFormatos;
        private DataGrid gridFormatos;
        private Button btnEliminar;

        protected Grupo grupo;
        private FiltroFormatosSignos filtroFormatos;
        private List<FormatosSignos> grupoFormatos;
        private Label lblLineas;
        private Label label2;
        private TextBox txtLineas;
        private TextBox txtGlobal;
        private GroupBox groupBox2;
        private Button btnSumapares;
        private Button btnTrios;
        private Button btnPares;
        private Button btnSacaFormatos;
        private Panel panel1;
        private Controls.MenuCondiciones menuCondiciones1;
        private int noRepPantalla;
        private Controls.ctrlAyuda ctrlAyuda1;
        private MainForm parentFrm;
        protected FormulariosHelper formHelper = new FormulariosHelper();

        public FormatosFrm(Grupo grupo, MainForm frm)
        {
            InitializeComponent();
            this.grupo = grupo;
            InicializaGrid();
            InicializaDatos();
            parentFrm=frm;
			
            compruebaPegar();
            ctrlAyuda1.TextoAyuda = "Un formato es una determinada\nsecuencia de signos y lo que\ncontrola esta condición es la\nrepetición o aparición de diferentes formatos";

        }

        public MainForm FormPadre
        {
            get{ return parentFrm; }
        }

        protected void InicializaGrid()
        {
            DataGridTableStyle tableStyle = new DataGridTableStyle();
            tableStyle.MappingName = "LineasFormatos";
            tableStyle.ColumnHeadersVisible = true;

            //Formato
            DataGridTextBoxColumn cs = new DataGridTextBoxColumn();
            cs.MappingName = "Formato";
            cs.HeaderText = "Formato";
            cs.Width = 80;
            tableStyle.GridColumnStyles.Add(cs);	
		
            //RangosApariciones
            cs = new DataGridTextBoxColumn();
            cs.MappingName = "RangoAparicion";
            cs.HeaderText = "Min-Max";
            cs.Width = 80;
            tableStyle.GridColumnStyles.Add(cs);

            gridFormatos.TableStyles.Add(tableStyle);
        }


        protected void InicializaDatos()
        {
            string nombreFiltro = Filtro.FormatosSignos.ToString();
            filtroFormatos = (FiltroFormatosSignos)grupo.GetFiltro( nombreFiltro );
            grupoFormatos = ObtenCopiaFormatos( filtroFormatos );
            ActualizaDatosPantalla( noRepPantalla );
        }


        protected List<FormatosSignos> ObtenCopiaFormatos( FiltroFormatosSignos filtro )
        {
            List<FormatosSignos> array_copia = new List<FormatosSignos>();			
			
            FormatosSignos formatos_copia;
            FormatoSignos formatolinea_copia;

            foreach( FormatosSignos formatos in filtro.FormatosSignos)
            {
                List<FormatoSignos> array_LineasFormatos = new List<FormatoSignos>();

                foreach(FormatoSignos formatolinea in formatos.LineasFormatos)
                {
                    formatolinea_copia = new FormatoSignos();
                    formatolinea_copia.Formato = formatolinea.Formato;
                    formatolinea_copia.RangoAparicion = formatolinea.RangoAparicion;
					
                    array_LineasFormatos.Add(formatolinea_copia);
                }

                formatos_copia  = new FormatosSignos();
                formatos_copia.LineasFormatos = array_LineasFormatos;
                formatos_copia.Lineas = formatos.Lineas;
                formatos_copia.Global = formatos.Global;
		
                array_copia.Add( formatos_copia );			
            }		
		
            return array_copia;
        }


        protected void ActualizaDatosPantalla( int noRep )
        {			
            if(grupoFormatos.Count > 0)
            {
                CargaDatosDataGrid( noRep );
                FormatosSignos formatos = grupoFormatos[noRep];
                txtLineas.Text = formatos.Lineas;
                txtGlobal.Text = formatos.Global;			
				
                lblNoFormatos.Text = (noRep + 1) + "/" + grupoFormatos.Count;
            }
            else
            {			
                LimpiaDatosDataGrid();
                txtLineas.Text = "";
                txtGlobal.Text = "";
                lblNoFormatos.Text = "1/1";
            }	
        }


        protected void CargaDatosDataGrid(int noRep)
        {
            List<FormatoSignos> lineasFormatos = grupoFormatos[noRep].LineasFormatos;
            //si hay menos de 30 filas crear...
            if(lineasFormatos.Count < 30)
            {
                for(int i = 0; i < 30 ; i++)
                {
                    FormatoSignos formatolinea = new FormatoSignos();
                    formatolinea.Formato = "";
                    formatolinea.RangoAparicion = "";
                    lineasFormatos.Add( formatolinea );				
                }				
            }
            DataSet miDataset = ObtenDataSetLineasFormatos( lineasFormatos );
            gridFormatos.SetDataBinding(miDataset, "LineasFormatos");
        }


        protected void LimpiaDatosDataGrid()
        {
            List<FormatoSignos> lineasFormatos = new List<FormatoSignos>();

            //crear datos en blanco			
            int noLineas = 30;

            for(int i = 0; i < noLineas; i++)
            {		
                FormatoSignos formatolinea = new FormatoSignos();
                formatolinea.Formato = "";
                formatolinea.RangoAparicion = "";

                lineasFormatos.Add( formatolinea );				
            }				

            DataSet miDataset = ObtenDataSetLineasFormatos( lineasFormatos );
            gridFormatos.SetDataBinding(miDataset, "LineasFormatos");		
        }


        protected DataSet ObtenDataSetLineasFormatos(List<FormatoSignos> lineasFormatosArray)
        {
            DataTable myDataTable = new DataTable("LineasFormatos");

            DataRow myDataRow;
			
            DataColumn myDataColumn = new DataColumn();
            myDataColumn.DataType = Type.GetType("System.String");
            myDataColumn.ColumnName = "Formato";
            myDataColumn.DefaultValue = "";
            myDataTable.Columns.Add(myDataColumn);	
		
            myDataColumn = new DataColumn();
            myDataColumn.DataType = Type.GetType("System.String");
            myDataColumn.ColumnName = "RangoAparicion";
            myDataColumn.DefaultValue = "";
            myDataTable.Columns.Add(myDataColumn);

            DataSet myDataSet = new DataSet();
            // Add the new DataTable to the DataSet.
            myDataSet.Tables.Add(myDataTable);

            //meter datos en el dataset
            for (int i = 0; i < lineasFormatosArray.Count; i++)
            {
                FormatoSignos formatolinea = lineasFormatosArray[i];

                myDataRow = myDataTable.NewRow();
                myDataRow["Formato"] = formatolinea.Formato;
                myDataRow["RangoAparicion"] = formatolinea.RangoAparicion;
				
                myDataTable.Rows.Add(myDataRow);
            }

            return myDataSet;		
        }


        protected bool TienePantallaDatos()
        {
            bool contieneDatos = true;
            if(ContieneGridDatos() == false)
            {
                contieneDatos = false;
            }
            return contieneDatos;		
        }

        protected bool ContieneGridDatos()
        {
            bool contieneDatos = false;
            DataSet ds = (DataSet)gridFormatos.DataSource;
            foreach(DataRow dr in ds.Tables[0].Rows)
            {
                if(	dr["Formato"].ToString() != "" && 
                   	dr["RangoAparicion"].ToString() != "")
                {
                    contieneDatos = true;
                    break;
                }		
            }
            return contieneDatos;
        }


        protected void CambiaFormatoSelecionado(int noRep)
        {
            //primero guardar datos de pantalla
            GuardarFormatosActual();
            noRepPantalla = noRep;

            //crear rep si no existe 			
            if( grupoFormatos.Count < noRep + 1 )
            {
                FormatosSignos formatos = new FormatosSignos();				
                grupoFormatos.Add( formatos );	
            }									
			
            //activa/desactiva boton "atras" si estamos en la primera relacion
            if( noRep == 0 )
            {
                btnRepPrev.Enabled = false;
            }
            else
            {
                btnRepPrev.Enabled = true;	
            }				

            ActualizaDatosPantalla( noRep );		
        }


        protected void GuardarFormatosActual()
        {
            FormatosSignos formatos;

            if( noRepPantalla < grupoFormatos.Count)
            {
                formatos = grupoFormatos[ noRepPantalla ];
                GuardarDatosFormatos( formatos );
            }
            else if( TienePantallaDatos() )
            {
                //existen datos en pantalla que se necesitan poner en nueva rep
                formatos = new FormatosSignos();
                grupoFormatos.Add( formatos );

                GuardarDatosFormatos( formatos );			
            }	
        }


        protected void GuardarDatosFormatos(FormatosSignos formatos)
        {
            if( TienePantallaDatos() )
            {
                formatos.LineasFormatos = ObtenDatosGrid();
                if (Utils.UtilidadesEntradasValores.SonTodosNumeros(txtLineas.Text.Trim()))
                {
                    formatos.Lineas = txtLineas.Text.Trim();
                }
                else
                {
                    formatos.Lineas = "";
                }

                if (Utils.UtilidadesEntradasValores.SonTodosNumeros(txtGlobal.Text.Trim()))
                {
                    formatos.Global = txtGlobal.Text.Trim();
                }
                else
                {
                    formatos.Global = "";
                }
            }			
        }


        protected void GuardarDatos()
        {
            GuardarFormatosActual();
            if( grupoFormatos.Count > 0 )
            {	
                //borrar ultima CP si no contiene datos
                if( NecesitaBorrarUltimoFormato() )
                {
                    BorrarFormatos( grupoFormatos.Count - 1 );				
                }
            }
			
            if(filtroFormatos.ContieneDatos == false && grupoFormatos.Count > 0)
            {
                //primera vez guardando datos. 
                //Activar condicion.
                filtroFormatos.ContieneDatos = true;
                filtroFormatos.IsActive = true;			
            }
			
            //guardar copia actualizada en filtro
            filtroFormatos.FormatosSignos = grupoFormatos;			
        }

        protected FiltroFormatosSignos ObtenerFiltroTemporal()
        {
            FiltroFormatosSignos filtroTemp = new FiltroFormatosSignos();
            List<FormatosSignos> grupoFormatosTemp = new List<FormatosSignos>();

            grupoFormatosTemp.AddRange(grupoFormatos);

            FormatosSignos formatos;

            if (noRepPantalla < grupoFormatosTemp.Count)
            {
                formatos = grupoFormatosTemp[noRepPantalla];
                GuardarDatosFormatos(formatos);
            }
            else if (TienePantallaDatos())
            {
                //existen datos en pantalla que se necesitan poner en nueva rep
                formatos = new FormatosSignos();
                grupoFormatosTemp.Add(formatos);

                GuardarDatosFormatos(formatos);
            }


            if (grupoFormatosTemp.Count > 0)
            {
                //borrar ultima CP si no contiene datos
                if (NecesitaBorrarUltimoFormatoTemporal(grupoFormatosTemp))
                {
                    grupoFormatosTemp.RemoveAt(grupoFormatosTemp.Count - 1);
                }
            }

            if (filtroTemp.ContieneDatos == false && grupoFormatosTemp.Count > 0)
            {
                //primera vez guardando datos. 
                //Activar condicion.
                filtroTemp.ContieneDatos = true;
                filtroTemp.IsActive = true;
            }

            //guardar copia actualizada en filtro
            filtroTemp.FormatosSignos = grupoFormatosTemp;

            return filtroTemp;
        }


        protected bool NecesitaBorrarUltimoFormato()
        {
            bool borrar = true;
			
            FormatosSignos formatos = grupoFormatos[ grupoFormatos.Count-1 ];
			
            foreach(FormatoSignos formato in formatos.LineasFormatos)
            {
                if(formato.Formato != "")
                {
                    //contiene datos
                    borrar = false;
                    break;
                }
            }
						
            return borrar;
        }

        protected bool NecesitaBorrarUltimoFormatoTemporal(List<FormatosSignos> arrayFormatosTemporal)
        {
            bool borrar = true;

            FormatosSignos formatos = (FormatosSignos)arrayFormatosTemporal[arrayFormatosTemporal.Count - 1];

            foreach (FormatoSignos formato in formatos.LineasFormatos)
            {
                if (formato.Formato != "")
                {
                    //contiene datos
                    borrar = false;
                    break;
                }
            }

            return borrar;
        }

        protected void BorrarFormatos( int noRep )
        {
            grupoFormatos.RemoveAt( noRep );		
        }


        protected List<FormatoSignos> ObtenDatosGrid()
        {
            List<FormatoSignos> arrayDatos = new List<FormatoSignos>();
            DataSet ds = (DataSet)gridFormatos.DataSource;

            foreach(DataRow dr in ds.Tables[0].Rows)
            {		
                string formato = dr["Formato"].ToString().Trim();
                string rangos = dr["RangoAparicion"].ToString().Trim();
				
                if(	CompruebaFomato(formato) && rangos != "")
                {
                    FormatoSignos formatolinea = new FormatoSignos();
                    formatolinea.Formato = formato;
                    formatolinea.RangoAparicion = rangos;

                    arrayDatos.Add(formatolinea);					
                }		
            }
            return arrayDatos;
        }
        private bool CompruebaFomato(string formato)
        {
            bool esValido = formato.Length <= VariablesGlobales.NumeroPartidos;

            if (esValido)
            {
                List<char> caracteresPermitidos = new List<char>(5);
                caracteresPermitidos.Add('1');
                caracteresPermitidos.Add('X');
                caracteresPermitidos.Add('2');
                caracteresPermitidos.Add('V');
                caracteresPermitidos.Add('*');

                for (int i = 0; i < formato.Length; i++)
                {
                    if (!caracteresPermitidos.Contains(formato[i]))
                    {
                        esValido = false;
                        break;
                    }
                }
            }
            return esValido;
        }

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormatosFrm));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lblNoFormatos = new System.Windows.Forms.Label();
            this.btnRepNext = new System.Windows.Forms.Button();
            this.btnRepPrev = new System.Windows.Forms.Button();
            this.gridFormatos = new System.Windows.Forms.DataGrid();
            this.btnEliminar = new System.Windows.Forms.Button();
            this.lblLineas = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtLineas = new System.Windows.Forms.TextBox();
            this.txtGlobal = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnSumapares = new System.Windows.Forms.Button();
            this.btnTrios = new System.Windows.Forms.Button();
            this.btnPares = new System.Windows.Forms.Button();
            this.btnSacaFormatos = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.ctrlAyuda1 = new Free1X2.UI.Controls.ctrlAyuda();
            this.menuCondiciones1 = new Free1X2.UI.Controls.MenuCondiciones();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridFormatos)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lblNoFormatos);
            this.groupBox1.Controls.Add(this.btnRepNext);
            this.groupBox1.Controls.Add(this.btnRepPrev);
            this.groupBox1.Location = new System.Drawing.Point(16, 8);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(136, 48);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            // 
            // lblNoFormatos
            // 
            this.lblNoFormatos.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNoFormatos.Location = new System.Drawing.Point(40, 16);
            this.lblNoFormatos.Name = "lblNoFormatos";
            this.lblNoFormatos.Size = new System.Drawing.Size(56, 23);
            this.lblNoFormatos.TabIndex = 2;
            this.lblNoFormatos.Text = "0/0";
            this.lblNoFormatos.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnRepNext
            // 
            this.btnRepNext.BackColor = System.Drawing.Color.LightSalmon;
            this.btnRepNext.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnRepNext.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRepNext.Image = ((System.Drawing.Image)(resources.GetObject("btnRepNext.Image")));
            this.btnRepNext.Location = new System.Drawing.Point(104, 15);
            this.btnRepNext.Name = "btnRepNext";
            this.btnRepNext.Size = new System.Drawing.Size(24, 23);
            this.btnRepNext.TabIndex = 1;
            this.btnRepNext.UseVisualStyleBackColor = false;
            this.btnRepNext.Click += new System.EventHandler(this.btnRepNext_Click);
            // 
            // btnRepPrev
            // 
            this.btnRepPrev.BackColor = System.Drawing.Color.Silver;
            this.btnRepPrev.Enabled = false;
            this.btnRepPrev.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnRepPrev.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRepPrev.Image = ((System.Drawing.Image)(resources.GetObject("btnRepPrev.Image")));
            this.btnRepPrev.Location = new System.Drawing.Point(8, 14);
            this.btnRepPrev.Name = "btnRepPrev";
            this.btnRepPrev.Size = new System.Drawing.Size(24, 23);
            this.btnRepPrev.TabIndex = 0;
            this.btnRepPrev.UseVisualStyleBackColor = false;
            this.btnRepPrev.Click += new System.EventHandler(this.btnRepPrev_Click);
            this.btnRepPrev.EnabledChanged += new System.EventHandler(this.btnRepPrev_EnabledChanged);
            // 
            // gridFormatos
            // 
            this.gridFormatos.AlternatingBackColor = System.Drawing.Color.PeachPuff;
            this.gridFormatos.BackColor = System.Drawing.Color.Bisque;
            this.gridFormatos.BackgroundColor = System.Drawing.Color.Bisque;
            this.gridFormatos.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.gridFormatos.CaptionVisible = false;
            this.gridFormatos.DataMember = "";
            this.gridFormatos.FlatMode = true;
            this.gridFormatos.GridLineColor = System.Drawing.Color.AntiqueWhite;
            this.gridFormatos.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.gridFormatos.Location = new System.Drawing.Point(16, 64);
            this.gridFormatos.Name = "gridFormatos";
            this.gridFormatos.ParentRowsBackColor = System.Drawing.Color.AntiqueWhite;
            this.gridFormatos.Size = new System.Drawing.Size(212, 288);
            this.gridFormatos.TabIndex = 3;
            // 
            // btnEliminar
            // 
            this.btnEliminar.BackColor = System.Drawing.Color.LightSalmon;
            this.btnEliminar.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnEliminar.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnEliminar.Image = ((System.Drawing.Image)(resources.GetObject("btnEliminar.Image")));
            this.btnEliminar.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnEliminar.Location = new System.Drawing.Point(269, 24);
            this.btnEliminar.Name = "btnEliminar";
            this.btnEliminar.Size = new System.Drawing.Size(120, 24);
            this.btnEliminar.TabIndex = 4;
            this.btnEliminar.Text = "Eliminar Actual";
            this.btnEliminar.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnEliminar.UseVisualStyleBackColor = false;
            this.btnEliminar.Click += new System.EventHandler(this.btnEliminar_Click);
            // 
            // lblLineas
            // 
            this.lblLineas.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLineas.Location = new System.Drawing.Point(237, 80);
            this.lblLineas.Name = "lblLineas";
            this.lblLineas.Size = new System.Drawing.Size(48, 21);
            this.lblLineas.TabIndex = 5;
            this.lblLineas.Text = "Líneas";
            this.lblLineas.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(237, 120);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(48, 21);
            this.label2.TabIndex = 6;
            this.label2.Text = "Global";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // txtLineas
            // 
            this.txtLineas.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtLineas.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtLineas.Location = new System.Drawing.Point(288, 80);
            this.txtLineas.Name = "txtLineas";
            this.txtLineas.Size = new System.Drawing.Size(101, 21);
            this.txtLineas.TabIndex = 7;
            // 
            // txtGlobal
            // 
            this.txtGlobal.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtGlobal.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtGlobal.Location = new System.Drawing.Point(288, 120);
            this.txtGlobal.Name = "txtGlobal";
            this.txtGlobal.Size = new System.Drawing.Size(101, 21);
            this.txtGlobal.TabIndex = 8;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btnSumapares);
            this.groupBox2.Controls.Add(this.btnTrios);
            this.groupBox2.Controls.Add(this.btnPares);
            this.groupBox2.Controls.Add(this.btnSacaFormatos);
            this.groupBox2.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.groupBox2.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox2.Location = new System.Drawing.Point(240, 168);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(160, 160);
            this.groupBox2.TabIndex = 9;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Utilidades de formatos";
            // 
            // btnSumapares
            // 
            this.btnSumapares.BackColor = System.Drawing.Color.DarkSalmon;
            this.btnSumapares.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnSumapares.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSumapares.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSumapares.Location = new System.Drawing.Point(11, 128);
            this.btnSumapares.Name = "btnSumapares";
            this.btnSumapares.Size = new System.Drawing.Size(138, 24);
            this.btnSumapares.TabIndex = 16;
            this.btnSumapares.Text = "Sumas Pares Nat.";
            this.btnSumapares.UseVisualStyleBackColor = false;
            this.btnSumapares.Click += new System.EventHandler(this.btnSumapares_Click);
            // 
            // btnTrios
            // 
            this.btnTrios.BackColor = System.Drawing.Color.DarkSalmon;
            this.btnTrios.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnTrios.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnTrios.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnTrios.Location = new System.Drawing.Point(11, 96);
            this.btnTrios.Name = "btnTrios";
            this.btnTrios.Size = new System.Drawing.Size(138, 24);
            this.btnTrios.TabIndex = 15;
            this.btnTrios.Text = "Tríos";
            this.btnTrios.UseVisualStyleBackColor = false;
            this.btnTrios.Click += new System.EventHandler(this.btnTrios_Click);
            // 
            // btnPares
            // 
            this.btnPares.BackColor = System.Drawing.Color.DarkSalmon;
            this.btnPares.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnPares.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnPares.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnPares.Location = new System.Drawing.Point(11, 64);
            this.btnPares.Name = "btnPares";
            this.btnPares.Size = new System.Drawing.Size(138, 24);
            this.btnPares.TabIndex = 14;
            this.btnPares.Text = "Pares";
            this.btnPares.UseVisualStyleBackColor = false;
            this.btnPares.Click += new System.EventHandler(this.btnPares_Click);
            // 
            // btnSacaFormatos
            // 
            this.btnSacaFormatos.BackColor = System.Drawing.Color.DarkSalmon;
            this.btnSacaFormatos.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnSacaFormatos.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSacaFormatos.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSacaFormatos.Location = new System.Drawing.Point(11, 32);
            this.btnSacaFormatos.Name = "btnSacaFormatos";
            this.btnSacaFormatos.Size = new System.Drawing.Size(138, 24);
            this.btnSacaFormatos.TabIndex = 13;
            this.btnSacaFormatos.Text = "Saca formatos";
            this.btnSacaFormatos.UseVisualStyleBackColor = false;
            this.btnSacaFormatos.Click += new System.EventHandler(this.btnSacaFormatos_Click);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.Bisque;
            this.panel1.Controls.Add(this.menuCondiciones1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 366);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(412, 48);
            this.panel1.TabIndex = 37;
            // 
            // ctrlAyuda1
            // 
            this.ctrlAyuda1.Location = new System.Drawing.Point(389, 0);
            this.ctrlAyuda1.Name = "ctrlAyuda1";
            this.ctrlAyuda1.Size = new System.Drawing.Size(20, 22);
            this.ctrlAyuda1.TabIndex = 38;
            this.ctrlAyuda1.TextoAyuda = "";
            // 
            // menuCondiciones1
            // 
            this.menuCondiciones1.Alineacion = Free1X2.alignment.Horizontal;
            this.menuCondiciones1.AutoSize = true;
            this.menuCondiciones1.BackColor = System.Drawing.Color.Bisque;
            this.menuCondiciones1.BotonAbrir = true;
            this.menuCondiciones1.BotonAbrirEnabled = true;
            this.menuCondiciones1.BotonBorrar = true;
            this.menuCondiciones1.BotonBorrarEnabled = true;
            this.menuCondiciones1.BotonCancelar = true;
            this.menuCondiciones1.BotonCancelarEnabled = true;
            this.menuCondiciones1.BotonCopiar = true;
            this.menuCondiciones1.BotonCopiarEnabled = true;
            this.menuCondiciones1.BotonEstadisticas = true;
            this.menuCondiciones1.BotonEstadisticasEnabled = true;
            this.menuCondiciones1.BotonGuardar = true;
            this.menuCondiciones1.BotonGuardarEnabled = true;
            this.menuCondiciones1.BotonOk = true;
            this.menuCondiciones1.BotonOkEnabled = true;
            this.menuCondiciones1.BotonPegar = true;
            this.menuCondiciones1.BotonPegarEnabled = true;
            this.menuCondiciones1.Location = new System.Drawing.Point(75, 8);
            this.menuCondiciones1.Name = "menuCondiciones1";
            this.menuCondiciones1.NumBotones = 8;
            this.menuCondiciones1.Size = new System.Drawing.Size(306, 36);
            this.menuCondiciones1.TabIndex = 2;
            this.menuCondiciones1.BOk += new System.EventHandler(this.menuCondiciones1_BOk);
            this.menuCondiciones1.BEstadisticas += new System.EventHandler(this.menuCondiciones1_BEstadisticas);
            this.menuCondiciones1.BGuardar += new System.EventHandler(this.menuCondiciones1_BGuardar);
            this.menuCondiciones1.BAbrir += new System.EventHandler(this.menuCondiciones1_BAbrir);
            this.menuCondiciones1.BPegar += new System.EventHandler(this.menuCondiciones1_BPegar);
            this.menuCondiciones1.BBorrar += new System.EventHandler(this.menuCondiciones1_BBorrar);
            this.menuCondiciones1.BCancelar += new System.EventHandler(this.menuCondiciones1_BCancelar);
            this.menuCondiciones1.BCopiar += new System.EventHandler(this.menuCondiciones1_BCopiar);
            // 
            // FormatosFrm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.Color.Bisque;
            this.ClientSize = new System.Drawing.Size(412, 414);
            this.ControlBox = false;
            this.Controls.Add(this.ctrlAyuda1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.txtGlobal);
            this.Controls.Add(this.txtLineas);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.lblLineas);
            this.Controls.Add(this.btnEliminar);
            this.Controls.Add(this.gridFormatos);
            this.Controls.Add(this.groupBox1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormatosFrm";
            this.Text = "Formatos (1,X,2,V,*)";
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridFormatos)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion

        private void btnRepPrev_Click(object sender, EventArgs e)
        {
            CambiaFormatoSelecionado( noRepPantalla - 1 );
        }

        private void btnRepNext_Click(object sender, EventArgs e)
        {
            if( TienePantallaDatos() )
            {
                CambiaFormatoSelecionado( noRepPantalla + 1 );
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            //si es la primera relacion
            if(noRepPantalla == 0)
            {
                //solo borrar si el formato ya esta guardado en memoria
                if(grupoFormatos.Count > 0)
                {
                    BorrarFormatos( noRepPantalla );
                }							
            }
            else
            {
                BorrarFormatos( noRepPantalla );
                noRepPantalla = noRepPantalla -1;			
            }	
		
            if(grupoFormatos.Count == 0)
            {
                //Pueden existir datos en pantalla que tenemos que borrar.
                //Inicializar rep y asignar. Al no cambiar de rep en pantalla,
                //los datos de esta rep en blanco apareceran en pantalla.
                FormatosSignos formatos = new FormatosSignos();				
                grupoFormatos.Add( formatos );			
            }

            //activa/desactiva boton "atras" si estamos en la primera relacion
            if( noRepPantalla == 0 )
            {
                btnRepPrev.Enabled = false;
            }
            else
            {
                btnRepPrev.Enabled = true;	
            }	
			
            ActualizaDatosPantalla( noRepPantalla );
        }

        private void btnSacaFormatos_Click(object sender, EventArgs e)
        {
            CalculoFormatosFrm sacaFormatos = new CalculoFormatosFrm();
            sacaFormatos.ShowDialog();
        }

        private void btnPares_Click(object sender, EventArgs e)
        {
            ParejasFrm parejasFrm = new ParejasFrm();
            parejasFrm.ShowDialog();
        }

        private void btnTrios_Click(object sender, EventArgs e)
        {
            TriosFrm triosFrm = new TriosFrm();
            triosFrm.ShowDialog();			
        }

        private void btnSumapares_Click(object sender, EventArgs e)
        {
            AnalizadorJPM analizadorJPMFrm = new AnalizadorJPM();
            analizadorJPMFrm.ShowDialog();			
        }

        private void btnRepPrev_EnabledChanged(object sender,EventArgs e)
        {
            formHelper.CambiarFondoBoton(btnRepPrev);
        }

        private void menuCondiciones1_BOk(object sender, EventArgs e)
        {
            GuardarDatos();
            FormPadre.analizador.GruposPartidos[FormPadre.pronosticos.GrupoPantalla].ActivaFiltro(filtroFormatos);
            CerrarVentana();
        }

        private void menuCondiciones1_BCancelar(object sender, EventArgs e)
        {
            CerrarVentana();
        }

        private void menuCondiciones1_BAbrir(object sender, EventArgs e)
        {
            GuardarDatos();
            if(filtroFormatos.FormatosSignos.Count>0)
            {
                if(MessageBox.Show("El filtro ya tiene datos introducidos. ¿Abrir igualmente?","Abrir condición",MessageBoxButtons.YesNo,MessageBoxIcon.Question,MessageBoxDefaultButton.Button2)==DialogResult.No)
                    return;
            }
            OpenFileDialog abreCombDialog = new OpenFileDialog();
            abreCombDialog.InitialDirectory = "Condiciones\\" ;
            abreCombDialog.Filter = "Formatos(*.fmt)|*.fmt|Formatos(*.xml)|*.xml|Todos los archivos (*.*)|*.*";
            if(abreCombDialog.ShowDialog() == DialogResult.OK)
                abrir(abreCombDialog.FileName);
        }

        private void menuCondiciones1_BGuardar(object sender, EventArgs e)
        {
            // Lo primero, guarda los datos de pantalla al filtro
            GuardarDatos();
            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.InitialDirectory = "Condiciones\\" ;
            saveDialog.Filter = "Formatos(*.fmt)|*.fmt|Formatos(*.xml)|*.xml|Todos los archivos (*.*)|*.*";
            if(saveDialog.ShowDialog() == DialogResult.OK)
                guardar(saveDialog.FileName);
        }

        private void abrir(string nombreArchivo)
        {
            //leer combinacion desde archivo
            ArchivoCondiciones archComb = new ArchivoCondiciones();
            if(archComb.AbrirArchivoCombinacion( nombreArchivo ))
            {
                grupo=archComb.LeeCondicion();
                filtroFormatos=(FiltroFormatosSignos)grupo.GetFiltro("FormatosSignos");
                grupoFormatos = ObtenCopiaFormatos( filtroFormatos );
                ActualizaDatosPantalla( noRepPantalla );
            }
        }

        private void guardar(string nombreArchivo)
        {
            ArchivoCondiciones archComb = new ArchivoCondiciones();
            archComb.NombreArchivo=nombreArchivo;
            if(filtroFormatos.FormatosSignos.Count>0)
            {
                filtroFormatos.ContieneDatos=true;
                filtroFormatos.IsActive=true;
            }
            archComb.GuardaArchivo(filtroFormatos);
        }

        private void menuCondiciones1_BBorrar(object sender, EventArgs e)
        {
            GuardarDatos();
            if(filtroFormatos.ContieneDatos)
            {
                if(MessageBox.Show("¿Borrar los datos del filtro?","Borrar condición",MessageBoxButtons.YesNo,MessageBoxIcon.Question,MessageBoxDefaultButton.Button2)==DialogResult.No)
                    return;
            }
            filtroFormatos=new FiltroFormatosSignos();
            grupoFormatos = ObtenCopiaFormatos( filtroFormatos );
            ActualizaDatosPantalla(0);
        }

        private void menuCondiciones1_BCopiar(object sender, EventArgs e)
        {
            // Lo primero, guarda los datos de pantalla al filtro
            GuardarDatos();
            // Crea un fichero temporal
            string nombreFichero=Application.StartupPath+"/Temp/"+"tmp.fmt";
            guardar(nombreFichero);
            menuCondiciones1.BotonPegarEnabled=true;
        }

        private void menuCondiciones1_BPegar(object sender, EventArgs e)
        {
            GuardarDatos();
            if(filtroFormatos.FormatosSignos.Count>0)
            {
                if(MessageBox.Show("El filtro ya tiene datos introducidos. ¿Abrir igualmente?","Abrir condición",MessageBoxButtons.YesNo,MessageBoxIcon.Question,MessageBoxDefaultButton.Button2)==DialogResult.No)
                    return;
            }
            string nombreFichero=Application.StartupPath+"/Temp/"+"tmp.fmt";
            abrir(nombreFichero);
        }

        private void compruebaPegar()
        {
            // Comprueba si el botón pegar es habilitable
            if(formHelper.ExisteFicheroTemporal("tmp.fmt"))
                menuCondiciones1.BotonPegarEnabled=true;
            else
                menuCondiciones1.BotonPegarEnabled=false;
        }
        private void CerrarVentana()
        {
            Close();
        }

        private void menuCondiciones1_BEstadisticas(object sender, EventArgs e)
        {
            FiltroFormatosSignos filtroTemp = ObtenerFiltroTemporal();
            CalculadorEstadisticas calc = new CalculadorEstadisticas();

            List<Estadistica> lista = calc.EstadisticasFiltro(filtroTemp, Application.StartupPath + "/Ganadoras/");

            Estadisticas.VisorEstadisticas visor = new Estadisticas.VisorEstadisticas(lista);

            visor.ShowDialog();
        }
    }
}
