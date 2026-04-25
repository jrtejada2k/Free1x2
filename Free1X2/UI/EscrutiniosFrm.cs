// created on 17/01/2004 at 11:43
// Free1X2 : Programa de quinielas "libre"
// Copyright (C) 2004 Luis Fernandez - luifer@onetel.net.uk
// Copyright (C) xfsf
// Copyright (C) 2008 Morrison - morrison.ne@gmail.com
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
using System.IO;
using System.Collections;
using System.Text;
using System.Windows.Forms;
using System.Data;
using System.Drawing;
using Free1X2.Escrutinio;
// using Free1X2.SVC_Actualizador; // TODO: Replace with modern HTTP client
using Free1X2.Utils;
using Free1X2.EntradaSalida;

using Free1X2.UI.Modern.Theming;
namespace Free1X2.UI
{
	public class EscrutiniosFrm : Form
	{
		private Button btnCancelar;
		private Button btnFileOrig;
		private DataGrid dgResultados;
		private Label lblTime;
		private Button btnComienzo;
		private Label lblFileRef;
		private Label labCG;
		private Button btnFileRef;
		
		private Escrutador escrutador;
		private DataSet resultadosDS;
		private DataSet dsJornadas;
		
		private string[] archivosComb;
		private string archivoRef = "";
		private DateTime hora0, hora9;
		private Label label1;
		private Button btnDisabSel;
		private Button btnEnableSel;
		private Button btnGrabaCols;
		private ListBox listaFicheros;
		private TextBox txtNoAciertos;
		private Button btnVerPremiadas;
        private CheckBox chkVerPremiadas;
		private TextBox txtColGanadora;
		private ToolTip toolTip1;
		private System.ComponentModel.IContainer components;
		private TabControl tabControl1;
		private TabPage tpSimple;
		private TabPage tbFichero;
		private TabPage tbTemporada;
		private ArrayList listaPremiadas=new ArrayList();
		private GroupBox groupBox3;
		private RadioButton rj1;
		private RadioButton rj2;
		private GroupBox groupBox2;
		private RadioButton rt2;
		private RadioButton rt4;
		private Label label4;
		private Label label5;
		private Label label3;
        private Label label2;
		private Button btnIntrJorn;
		private Button btnIntrTemp;
		private Button btnVerArch;
		private TextBox txtNombreArchBase;
		private Label label7;
		private Panel panel1;
		private int tipoEscrutinio;
		private OpenFileDialog fd;
        private ListBox lstTemporadas;
		private FolderBrowserDialog fbd;
		private Button btnBuscarCarpeta;
        private Label lblCarpeta;
		private Button btnPosiblesPremios;
	    private Label label6;
        private Label label8;
        private Label label9;
        private Button btnActualizaCG;
        private int noPartidos;

		public EscrutiniosFrm()
		{
            try
            {
                noPartidos = VariablesGlobales.NumeroPartidos;
            }
            catch
            {
                noPartidos = 14;
            }

			InitializeComponent();
            txtNoAciertos.Text = "10-" + noPartidos;
			InicializaGridResultados();
			crearDataset();
            GenerarCasillasPartidos();
            FormulariosHelper fH = new FormulariosHelper();
            fH.Traducir(this);
		}
        protected override void OnLoad(System.EventArgs e)
        {
            base.OnLoad(e);
            ModernTheme.ApplyToForm(this);
        }

        protected void GenerarCasillasPartidos()
        {
            string nombre = "txtCG";
            int x = 144;
            int y = 24;
            for (int i = 0; i < noPartidos; i++)
            {
                TextBox t = new TextBox();
                Label l = new Label();
                //TextBox
                t.Name = nombre + Convert.ToString(i + 1);
                t.BorderStyle = BorderStyle.FixedSingle;
                t.Location = new Point(x, y);
                t.Size = new Size(20, 20);
                t.TextAlign = HorizontalAlignment.Center;
                t.MaxLength = 1;
                t.Leave += resultadoCambiado;

                //Label
                l.Font = new Font("Microsoft Sans Serif", 8F, FontStyle.Bold, GraphicsUnit.Point, 0);
                l.Location = new Point(x, y - 21);
                l.Name = "lblCG" + Convert.ToString(i + 1);
                l.Size = new Size(17, 20);
                l.TabIndex = 38;
                l.Text = Convert.ToString(i + 1);
                l.TextAlign = ContentAlignment.MiddleCenter;
                l.MouseUp += partidoClicado;

                tpSimple.Controls.Add(t);
                tpSimple.Controls.Add(l);

                x += 21;
            }
        }

		protected void InicializaResultadosDataSet()
		{
		    resultadosDS = new DataSet();

			DataTable newTable = new DataTable("Resultados");
			newTable.Columns.Add("Seleccionado", typeof(bool));
			newTable.Columns.Add("LineaID", typeof(int));
			newTable.Columns.Add("Columna", typeof(string));
			newTable.Columns.Add("Archivo", typeof(string));
			for(int i=0;i<=VariablesGlobales.NumeroPartidos;i++)
			{
			    string nombre = "P"+i;
			    newTable.Columns.Add(nombre, typeof(int));
			}
		    newTable.Columns.Add("Ac. Totales", typeof(string));

			resultadosDS.Tables.Add(newTable);		
		}
		
		protected void InicializaGridResultados()
		{			
			DataGridTableStyle tableStyle = new DataGridTableStyle();
			tableStyle.MappingName = "Resultados";
			tableStyle.ColumnHeadersVisible = true;
			
			//columna seleccionada
			DataGridBoolColumn csBool = new DataGridBoolColumn();
			csBool.MappingName = "Seleccionado";
			csBool.HeaderText = "";
			csBool.Width = 40;
			csBool.TrueValue = true;
			csBool.FalseValue = false;
			tableStyle.GridColumnStyles.Add(csBool);

			// Crear Columnas para cada premio			            

		    //Archivo
			DataGridTextBoxColumn cs = new DataGridTextBoxColumn();
			cs.MappingName = "Archivo";
			cs.HeaderText = "Archivo";
			cs.Width = 150;
			tableStyle.GridColumnStyles.Add(cs);

			//NoJornada
			cs = new DataGridTextBoxColumn();
			cs.MappingName = "LineaID";
			cs.HeaderText = "No";
			cs.Width = 40;
			tableStyle.GridColumnStyles.Add(cs);

			//Columna
			cs = new DataGridTextBoxColumn();
			cs.MappingName = "Columna";
			cs.HeaderText = "Columna";
			cs.Width = 100;
			tableStyle.GridColumnStyles.Add(cs);

			RangosHelper rangos = new RangosHelper();
			int[] colAciertos = rangos.ObtenIntArray( txtNoAciertos.Text );
			
			for(int i = colAciertos.Length; i > 0; i--)
			{
				cs = new DataGridTextBoxColumn();
				cs.MappingName = "P" + colAciertos[i-1];
				cs.HeaderText = colAciertos[i-1].ToString();
				cs.Width = 40;
				tableStyle.GridColumnStyles.Add(cs);			
			}
			tableStyle.AllowSorting=true;
            //Los totales por cada columna hay que inicializarlos aquí
            //Aciertos Totales
            cs = new DataGridTextBoxColumn();
            cs.MappingName = "Ac. Totales";
            cs.HeaderText = "Ac. Totales";
            cs.Width = 100;
            tableStyle.GridColumnStyles.Add(cs);

			//primero borrar los TableStyles existentes
			dgResultados.TableStyles.Clear();
		    dgResultados.TableStyles.Add(tableStyle);	
		}		
		
		protected void GridDataBind()
		{
			dgResultados.SetDataBinding(resultadosDS, "Resultados");			
		}
		
		protected void RealizaEscrutinio()
		{
			int[] premiosGlobales=new int[VariablesGlobales.NumeroPartidos+1];
			listaPremiadas.Clear();
			RangosHelper rangos = new RangosHelper();
			int[] colAciertos = rangos.ObtenIntArray( txtNoAciertos.Text );
			hora0 = DateTime.Now;
			lblTime.Text = "Calculando...";

			InicializaResultadosDataSet();
			InicializaGridResultados();
			GridDataBind();
			if(tipoEscrutinio==3)
			{
				string numeros="0123456789";
				int dt, dj;
				if(rt4.Checked) dt=4; else dt=2;
				if(rj2.Checked) dj=2; else dj=1;
				ArrayList listaInicialArchivos=new ArrayList();
				int temporada, jornada;
				string temp, jorn;
				string consulta="";
				// Creamos un dataView para seleccionar las temporadas en el dataset
				for(int i=0;i<lstTemporadas.SelectedIndices.Count;i++)
				{
					int x = lstTemporadas.SelectedIndices[i];
					temporada=Convert.ToInt16(lstTemporadas.Items[x]);
					consulta+=" or Temp="+temporada;
				}
				if(consulta.Length>0) consulta=consulta.Substring(4);
				DataView dv=new DataView(dsJornadas.Tables[0]);
				dv.RowFilter=consulta;
				if(dv.Count==0) return;
				// Busca todas las jornadas
				DirectoryInfo di=new DirectoryInfo(lblCarpeta.Text+"/");
				for(int i=0;i<dv.Count;i++)
				{
					temporada=Convert.ToInt16(dv[i]["Temp"]);
					jornada=Convert.ToInt16(dv[i]["Jorn"]);
					temp=temporada.ToString("0000");
					if(dt==2) temp=temp.Substring(2);
					if(dj==2) jorn=jornada.ToString("00"); else jorn=jornada.ToString();
					consulta=txtNombreArchBase.Text;
					consulta=consulta.Replace("/t",temp);
					consulta=consulta.Replace("/j",jorn);
					FileInfo[] fi=di.GetFiles(consulta);
					for(int j=0;j<fi.Length;j++)
					{
						listaInicialArchivos.Add(consulta);
					}
				}
				int posTemp=txtNombreArchBase.Text.IndexOf("/t");
				int posJorn=txtNombreArchBase.Text.IndexOf("/j");
				string archivo;
				for(int j=0;j<listaInicialArchivos.Count;j++)
				{
					archivo=(string)listaInicialArchivos[j];
					if(posTemp>posJorn)
					{
						jorn=archivo.Substring(posJorn,2);
						if(numeros.IndexOf(jorn.Substring(1))>=0)
							dj=2;
						else
						{
							dj=1;
							jorn=jorn.Substring(0,1);
						}
						jornada=Convert.ToInt16(jorn);
						temporada=Convert.ToInt16(archivo.Substring(posTemp+dj-2,dt));
					}
					else
					{
						temporada=Convert.ToInt16(archivo.Substring(posTemp,dt));
						jorn=archivo.Substring(posJorn+dt-2,2);
						if(numeros.IndexOf(jorn.Substring(1))<0)
						{
						    jorn=jorn.Substring(0,1);
						}
						jornada=Convert.ToInt16(jorn);
					}
					dv.RowFilter="Temp="+temporada+" and Jorn="+jornada;
					string colGan = dv[0]["Quiniela"].ToString();
					// Escrutinio
					escrutador = new Escrutador(colAciertos);
					escrutador.ArchivoColumnas = archivo;
					escrutador.AñadirAGanadoras = chkVerPremiadas.Checked;									
					escrutador.EscrutaCombConColumna( colGan, resultadosDS, Path.GetFileName(archivo));
					if(escrutador.AñadirAGanadoras)
					{
						for(int i=0; i < escrutador.ListaPremiadas.Count; i++)
						{
							listaPremiadas.Add(escrutador.ListaPremiadas[i]);
						}
					}
					sumarPremios(ref premiosGlobales, escrutador.PremiosTotales);
				}
			}
			else
			{
				for(int i=0;i<archivosComb.Length;i++)
				{
					escrutador = new Escrutador(colAciertos);
					escrutador.ArchivoColumnas = archivosComb[i];
					escrutador.AñadirAGanadoras = chkVerPremiadas.Checked;									
				
					if( tipoEscrutinio==1)
					{
						string colGan = ObtenColGanadora();
						escrutador.EscrutaCombConColumna( colGan, resultadosDS, archivosComb[i]);
					}
					else if(tipoEscrutinio==2)
					{
						escrutador.EscrutaCombConTemporada( lblFileRef.Tag.ToString(), resultadosDS, archivosComb[i]);
					}

					if(escrutador.AñadirAGanadoras)
					{
						for(int j=0; j < escrutador.ListaPremiadas.Count; j++)
						{
							listaPremiadas.Add(escrutador.ListaPremiadas[j]);
						}
					}
					sumarPremios(ref premiosGlobales, escrutador.PremiosTotales);
				}
			}
			// Añade el resumen de premiadas
			escrutador.AñadirPremiosGlobales(premiosGlobales);

			hora9 = DateTime.Now;
			string tiempo = "Final = " + (hora9 - hora0);

			if(tiempo.Length >= 18)
			{
				tiempo = tiempo.Substring(0,18);
			}
			lblTime.Text = tiempo;
			btnComienzo.Enabled = true;
		}

		private void sumarPremios(ref int[] premiosGlobales, int[] premios)
		{
			for(int i=0;i<=VariablesGlobales.NumeroPartidos;i++)
			{
				premiosGlobales[i]+=premios[i];
			}
		}

		protected string ObtenColGanadora()
		{
            string col = "";
            for (int i = 0; i < VariablesGlobales.NumeroPartidos; i++)
            {
                TextBox t = buscarCuadroPartido(i + 1);
                if (t.Text.Trim() != "")
                {
                    col += t.Text.Trim();
                }
                else
                {
                    col += "*";
                }
            }

            return col;	
		}
				
		protected bool SonDatosValidos() 
		{
			bool datosValidos = true;
			string msg = "";
			
			if(archivosComb==null && tipoEscrutinio!=3)
			{
				msg="falta fichero a escrutar";
			}
			else if (tipoEscrutinio==1)
			{
				//obtener columna ganadora sin *
				string col = ObtenColGanadora().Replace("*","");
				if(col == "")
				{
					msg="falta columna ganadora";
				}
			}
			else if (tipoEscrutinio==2 && (lblFileRef.Text=="")) 
			{
				msg="falta fichero referencia";
			}
			else if (tipoEscrutinio==3)
			{
				if(txtNombreArchBase.Text.Length==0)
					msg="Falta plantilla de nombre de fichero.";
				else if(lblCarpeta.Text.Length==0)
					msg="Falta la carpeta de los ficheros.";
				else if(txtNombreArchBase.Text.IndexOf("/t")<0)
					msg="No se ha puesto el indicador de temporada (/t).";
				else if(txtNombreArchBase.Text.IndexOf("/j")<0)
					msg="No se ha puesto el indicador de jornada (/j).";
			}

			if (msg == "" && tipoEscrutinio==1)
			{
				string cg = ObtenColGanadora();
				if( cg.Length != VariablesGlobales.NumeroPartidos) msg="número de partidos incorrecto";
				else
				foreach(char ch in cg) 
				{
					if(ch != 'x' && ch != 'X' && ch != '1' && ch != '2' && ch != '*' && ch != 'S') {
						msg = "La C.G. solo puede contener los caracteres: 1,X,2,*,S";
						break;
					}				
				}
			}
			
			if (msg!="")
			{
				MessageBox.Show (msg, "Datos insuficientes!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				datosValidos = false;
			}
			return datosValidos;
		}
		
		
		#region designer
		
		
		void InitializeComponent() 
		{
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EscrutiniosFrm));
            this.btnFileRef = new System.Windows.Forms.Button();
            this.labCG = new System.Windows.Forms.Label();
            this.lblFileRef = new System.Windows.Forms.Label();
            this.btnComienzo = new System.Windows.Forms.Button();
            this.lblTime = new System.Windows.Forms.Label();
            this.dgResultados = new System.Windows.Forms.DataGrid();
            this.btnFileOrig = new System.Windows.Forms.Button();
            this.btnCancelar = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.txtNoAciertos = new System.Windows.Forms.TextBox();
            this.btnDisabSel = new System.Windows.Forms.Button();
            this.btnEnableSel = new System.Windows.Forms.Button();
            this.btnGrabaCols = new System.Windows.Forms.Button();
            this.listaFicheros = new System.Windows.Forms.ListBox();
            this.btnVerPremiadas = new System.Windows.Forms.Button();
            this.chkVerPremiadas = new System.Windows.Forms.CheckBox();
            this.txtColGanadora = new System.Windows.Forms.TextBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.btnIntrJorn = new System.Windows.Forms.Button();
            this.btnIntrTemp = new System.Windows.Forms.Button();
            this.btnVerArch = new System.Windows.Forms.Button();
            this.lstTemporadas = new System.Windows.Forms.ListBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tpSimple = new System.Windows.Forms.TabPage();
            this.label9 = new System.Windows.Forms.Label();
            this.tbFichero = new System.Windows.Forms.TabPage();
            this.tbTemporada = new System.Windows.Forms.TabPage();
            this.label8 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.btnBuscarCarpeta = new System.Windows.Forms.Button();
            this.lblCarpeta = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.rj1 = new System.Windows.Forms.RadioButton();
            this.rj2 = new System.Windows.Forms.RadioButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.rt2 = new System.Windows.Forms.RadioButton();
            this.rt4 = new System.Windows.Forms.RadioButton();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtNombreArchBase = new System.Windows.Forms.TextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.fd = new System.Windows.Forms.OpenFileDialog();
            this.fbd = new System.Windows.Forms.FolderBrowserDialog();
            this.btnPosiblesPremios = new System.Windows.Forms.Button();
            this.btnActualizaCG = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgResultados)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.tpSimple.SuspendLayout();
            this.tbFichero.SuspendLayout();
            this.tbTemporada.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnFileRef
            // 
            this.btnFileRef.BackColor = System.Drawing.Color.DarkSalmon;
            this.btnFileRef.FlatAppearance.BorderSize = 0;
            this.btnFileRef.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnFileRef.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnFileRef.Image = ((System.Drawing.Image)(resources.GetObject("btnFileRef.Image")));
            this.btnFileRef.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnFileRef.Location = new System.Drawing.Point(8, 16);
            this.btnFileRef.Name = "btnFileRef";
            this.btnFileRef.Size = new System.Drawing.Size(139, 32);
            this.btnFileRef.TabIndex = 9;
            this.btnFileRef.Text = "Fichero referencia";
            this.btnFileRef.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnFileRef.UseVisualStyleBackColor = false;
            this.btnFileRef.Click += new System.EventHandler(this.BtnFileRefClick);
            // 
            // labCG
            // 
            this.labCG.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labCG.ForeColor = System.Drawing.Color.Maroon;
            this.labCG.Location = new System.Drawing.Point(8, 24);
            this.labCG.Name = "labCG";
            this.labCG.Size = new System.Drawing.Size(135, 17);
            this.labCG.TabIndex = 7;
            this.labCG.Text = "Columna Ganadora";
            // 
            // lblFileRef
            // 
            this.lblFileRef.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFileRef.Location = new System.Drawing.Point(162, 24);
            this.lblFileRef.Name = "lblFileRef";
            this.lblFileRef.Size = new System.Drawing.Size(352, 16);
            this.lblFileRef.TabIndex = 10;
            this.lblFileRef.Text = "(selecciona)";
            this.lblFileRef.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnComienzo
            // 
            this.btnComienzo.BackColor = System.Drawing.Color.DarkSalmon;
            this.btnComienzo.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnComienzo.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnComienzo.Image = ((System.Drawing.Image)(resources.GetObject("btnComienzo.Image")));
            this.btnComienzo.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnComienzo.Location = new System.Drawing.Point(16, 280);
            this.btnComienzo.Name = "btnComienzo";
            this.btnComienzo.Size = new System.Drawing.Size(128, 32);
            this.btnComienzo.TabIndex = 0;
            this.btnComienzo.Text = "Escrutar!";
            this.btnComienzo.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnComienzo.UseVisualStyleBackColor = false;
            this.btnComienzo.Click += new System.EventHandler(this.BtnComienzoClick);
            // 
            // lblTime
            // 
            this.lblTime.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTime.Location = new System.Drawing.Point(160, 280);
            this.lblTime.Name = "lblTime";
            this.lblTime.Size = new System.Drawing.Size(279, 32);
            this.lblTime.TabIndex = 12;
            this.lblTime.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // dgResultados
            // 
            this.dgResultados.BackgroundColor = System.Drawing.Color.White;
            this.dgResultados.CaptionBackColor = System.Drawing.Color.DarkSalmon;
            this.dgResultados.DataMember = "";
            this.dgResultados.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.dgResultados.Location = new System.Drawing.Point(16, 320);
            this.dgResultados.Name = "dgResultados";
            this.dgResultados.ReadOnly = true;
            this.dgResultados.Size = new System.Drawing.Size(568, 221);
            this.dgResultados.TabIndex = 14;
            this.dgResultados.MouseUp += new System.Windows.Forms.MouseEventHandler(this.DgResultados_Click);
            // 
            // btnFileOrig
            // 
            this.btnFileOrig.BackColor = System.Drawing.Color.DarkSalmon;
            this.btnFileOrig.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnFileOrig.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnFileOrig.Image = ((System.Drawing.Image)(resources.GetObject("btnFileOrig.Image")));
            this.btnFileOrig.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnFileOrig.Location = new System.Drawing.Point(16, 40);
            this.btnFileOrig.Name = "btnFileOrig";
            this.btnFileOrig.Size = new System.Drawing.Size(138, 32);
            this.btnFileOrig.TabIndex = 8;
            this.btnFileOrig.Text = "Fichero a escrutar";
            this.btnFileOrig.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnFileOrig.UseVisualStyleBackColor = false;
            this.btnFileOrig.Click += new System.EventHandler(this.BtnFileOrigClick);
            // 
            // btnCancelar
            // 
            this.btnCancelar.BackColor = System.Drawing.Color.DarkSalmon;
            this.btnCancelar.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnCancelar.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancelar.Image = ((System.Drawing.Image)(resources.GetObject("btnCancelar.Image")));
            this.btnCancelar.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCancelar.Location = new System.Drawing.Point(495, 547);
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.Size = new System.Drawing.Size(88, 24);
            this.btnCancelar.TabIndex = 1;
            this.btnCancelar.Text = "Cancelar";
            this.btnCancelar.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnCancelar.UseVisualStyleBackColor = false;
            this.btnCancelar.Click += new System.EventHandler(this.BtnCancelarClick);
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.Maroon;
            this.label1.Location = new System.Drawing.Point(24, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(90, 21);
            this.label1.TabIndex = 15;
            this.label1.Text = "No Aciertos";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // txtNoAciertos
            // 
            this.txtNoAciertos.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtNoAciertos.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtNoAciertos.Location = new System.Drawing.Point(120, 8);
            this.txtNoAciertos.Name = "txtNoAciertos";
            this.txtNoAciertos.Size = new System.Drawing.Size(80, 21);
            this.txtNoAciertos.TabIndex = 16;
            this.txtNoAciertos.Text = "10-14";
            // 
            // btnDisabSel
            // 
            this.btnDisabSel.BackColor = System.Drawing.Color.DarkSalmon;
            this.btnDisabSel.Enabled = false;
            this.btnDisabSel.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnDisabSel.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDisabSel.Image = ((System.Drawing.Image)(resources.GetObject("btnDisabSel.Image")));
            this.btnDisabSel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnDisabSel.Location = new System.Drawing.Point(142, 547);
            this.btnDisabSel.Name = "btnDisabSel";
            this.btnDisabSel.Size = new System.Drawing.Size(140, 24);
            this.btnDisabSel.TabIndex = 17;
            this.btnDisabSel.Text = "Deseleccionar Todas";
            this.btnDisabSel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnDisabSel.UseVisualStyleBackColor = false;
            this.btnDisabSel.Click += new System.EventHandler(this.btnDisabSel_Click);
            // 
            // btnEnableSel
            // 
            this.btnEnableSel.BackColor = System.Drawing.Color.DarkSalmon;
            this.btnEnableSel.Enabled = false;
            this.btnEnableSel.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnEnableSel.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnEnableSel.Image = ((System.Drawing.Image)(resources.GetObject("btnEnableSel.Image")));
            this.btnEnableSel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnEnableSel.Location = new System.Drawing.Point(15, 547);
            this.btnEnableSel.Name = "btnEnableSel";
            this.btnEnableSel.Size = new System.Drawing.Size(126, 24);
            this.btnEnableSel.TabIndex = 18;
            this.btnEnableSel.Text = "Seleccionar Todas";
            this.btnEnableSel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnEnableSel.UseVisualStyleBackColor = false;
            this.btnEnableSel.Click += new System.EventHandler(this.btnEnableSel_Click);
            // 
            // btnGrabaCols
            // 
            this.btnGrabaCols.BackColor = System.Drawing.Color.DarkSalmon;
            this.btnGrabaCols.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnGrabaCols.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnGrabaCols.Image = ((System.Drawing.Image)(resources.GetObject("btnGrabaCols.Image")));
            this.btnGrabaCols.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnGrabaCols.Location = new System.Drawing.Point(283, 547);
            this.btnGrabaCols.Name = "btnGrabaCols";
            this.btnGrabaCols.Size = new System.Drawing.Size(104, 24);
            this.btnGrabaCols.TabIndex = 19;
            this.btnGrabaCols.Text = "Grabar cols";
            this.btnGrabaCols.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnGrabaCols.UseVisualStyleBackColor = false;
            this.btnGrabaCols.Click += new System.EventHandler(this.btnGrabaCols_Click);
            // 
            // listaFicheros
            // 
            this.listaFicheros.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.listaFicheros.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listaFicheros.Items.AddRange(new object[] {
            "(selecciona)"});
            this.listaFicheros.Location = new System.Drawing.Point(160, 40);
            this.listaFicheros.Name = "listaFicheros";
            this.listaFicheros.ScrollAlwaysVisible = true;
            this.listaFicheros.SelectionMode = System.Windows.Forms.SelectionMode.None;
            this.listaFicheros.Size = new System.Drawing.Size(328, 93);
            this.listaFicheros.Sorted = true;
            this.listaFicheros.TabIndex = 20;
            this.listaFicheros.TabStop = false;
            // 
            // btnVerPremiadas
            // 
            this.btnVerPremiadas.BackColor = System.Drawing.Color.DarkSalmon;
            this.btnVerPremiadas.Enabled = false;
            this.btnVerPremiadas.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnVerPremiadas.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnVerPremiadas.Image = ((System.Drawing.Image)(resources.GetObject("btnVerPremiadas.Image")));
            this.btnVerPremiadas.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnVerPremiadas.Location = new System.Drawing.Point(388, 547);
            this.btnVerPremiadas.Name = "btnVerPremiadas";
            this.btnVerPremiadas.Size = new System.Drawing.Size(106, 24);
            this.btnVerPremiadas.TabIndex = 21;
            this.btnVerPremiadas.Text = "Ver Premiadas";
            this.btnVerPremiadas.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnVerPremiadas.UseVisualStyleBackColor = false;
            this.btnVerPremiadas.Click += new System.EventHandler(this.btnVerPremiadas_Click);
            // 
            // chkVerPremiadas
            // 
            this.chkVerPremiadas.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkVerPremiadas.Location = new System.Drawing.Point(216, 8);
            this.chkVerPremiadas.Name = "chkVerPremiadas";
            this.chkVerPremiadas.Size = new System.Drawing.Size(211, 24);
            this.chkVerPremiadas.TabIndex = 21;
            this.chkVerPremiadas.Text = "Activar Ver Premiadas";
            // 
            // txtColGanadora
            // 
            this.txtColGanadora.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtColGanadora.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtColGanadora.Location = new System.Drawing.Point(200, 56);
            this.txtColGanadora.MaxLength = 14;
            this.txtColGanadora.Name = "txtColGanadora";
            this.txtColGanadora.Size = new System.Drawing.Size(112, 21);
            this.txtColGanadora.TabIndex = 50;
            this.toolTip1.SetToolTip(this.txtColGanadora, "Introducir columna manualmente");
            this.txtColGanadora.Leave += new System.EventHandler(this.columnaCambiada);
            // 
            // btnIntrJorn
            // 
            this.btnIntrJorn.BackColor = System.Drawing.Color.LightSalmon;
            this.btnIntrJorn.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnIntrJorn.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnIntrJorn.Location = new System.Drawing.Point(288, 32);
            this.btnIntrJorn.Name = "btnIntrJorn";
            this.btnIntrJorn.Size = new System.Drawing.Size(22, 20);
            this.btnIntrJorn.TabIndex = 26;
            this.btnIntrJorn.TabStop = false;
            this.btnIntrJorn.Text = "/j";
            this.toolTip1.SetToolTip(this.btnIntrJorn, "Introducir jornada");
            this.btnIntrJorn.UseVisualStyleBackColor = false;
            this.btnIntrJorn.Click += new System.EventHandler(this.incluirPrefijo_click);
            // 
            // btnIntrTemp
            // 
            this.btnIntrTemp.BackColor = System.Drawing.Color.LightSalmon;
            this.btnIntrTemp.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnIntrTemp.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnIntrTemp.Location = new System.Drawing.Point(264, 32);
            this.btnIntrTemp.Name = "btnIntrTemp";
            this.btnIntrTemp.Size = new System.Drawing.Size(22, 20);
            this.btnIntrTemp.TabIndex = 25;
            this.btnIntrTemp.TabStop = false;
            this.btnIntrTemp.Text = "/t";
            this.toolTip1.SetToolTip(this.btnIntrTemp, "Introducir temporada");
            this.btnIntrTemp.UseVisualStyleBackColor = false;
            this.btnIntrTemp.Click += new System.EventHandler(this.incluirPrefijo_click);
            // 
            // btnVerArch
            // 
            this.btnVerArch.BackColor = System.Drawing.Color.LightSalmon;
            this.btnVerArch.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnVerArch.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnVerArch.Location = new System.Drawing.Point(240, 32);
            this.btnVerArch.Name = "btnVerArch";
            this.btnVerArch.Size = new System.Drawing.Size(22, 20);
            this.btnVerArch.TabIndex = 24;
            this.btnVerArch.TabStop = false;
            this.btnVerArch.Text = "?";
            this.toolTip1.SetToolTip(this.btnVerArch, "Ver archivos");
            this.btnVerArch.UseVisualStyleBackColor = false;
            this.btnVerArch.Click += new System.EventHandler(this.btnVerArch_Click);
            // 
            // lstTemporadas
            // 
            this.lstTemporadas.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lstTemporadas.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lstTemporadas.ItemHeight = 12;
            this.lstTemporadas.Location = new System.Drawing.Point(456, 16);
            this.lstTemporadas.Name = "lstTemporadas";
            this.lstTemporadas.ScrollAlwaysVisible = true;
            this.lstTemporadas.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lstTemporadas.Size = new System.Drawing.Size(98, 74);
            this.lstTemporadas.TabIndex = 34;
            this.lstTemporadas.TabStop = false;
            this.toolTip1.SetToolTip(this.lstTemporadas, "Marcar para seleccionar temporadas");
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tpSimple);
            this.tabControl1.Controls.Add(this.tbFichero);
            this.tabControl1.Controls.Add(this.tbTemporada);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Top;
            this.tabControl1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabControl1.ItemSize = new System.Drawing.Size(90, 18);
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.ShowToolTips = true;
            this.tabControl1.Size = new System.Drawing.Size(566, 128);
            this.tabControl1.TabIndex = 54;
            this.tabControl1.TabStop = false;
            this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.tabControl1_SelectedIndexChanged);
            // 
            // tpSimple
            // 
            this.tpSimple.BackColor = System.Drawing.Color.Bisque;
            this.tpSimple.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tpSimple.Controls.Add(this.btnActualizaCG);
            this.tpSimple.Controls.Add(this.label9);
            this.tpSimple.Controls.Add(this.labCG);
            this.tpSimple.Controls.Add(this.txtColGanadora);
            this.tpSimple.Location = new System.Drawing.Point(4, 22);
            this.tpSimple.Name = "tpSimple";
            this.tpSimple.Size = new System.Drawing.Size(558, 102);
            this.tpSimple.TabIndex = 0;
            this.tpSimple.Tag = "1";
            this.tpSimple.Text = "Escrutinio simple";
            this.tpSimple.ToolTipText = "Escruta fichero/s contra una columna";
            // 
            // label9
            // 
            this.label9.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.ForeColor = System.Drawing.Color.Maroon;
            this.label9.Location = new System.Drawing.Point(9, 37);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(94, 59);
            this.label9.TabIndex = 58;
            this.label9.Text = "1: Local\r\nX: Empate\r\n2: Visitante\r\n*: Comodín\r\nS: Suspendido";
            // 
            // tbFichero
            // 
            this.tbFichero.BackColor = System.Drawing.Color.Bisque;
            this.tbFichero.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbFichero.Controls.Add(this.btnFileRef);
            this.tbFichero.Controls.Add(this.lblFileRef);
            this.tbFichero.Location = new System.Drawing.Point(4, 22);
            this.tbFichero.Name = "tbFichero";
            this.tbFichero.Size = new System.Drawing.Size(558, 102);
            this.tbFichero.TabIndex = 1;
            this.tbFichero.Tag = "2";
            this.tbFichero.Text = "Escrutinio contra fichero";
            this.tbFichero.ToolTipText = "Escruta fichero/s contra las columnas de otro fichero";
            // 
            // tbTemporada
            // 
            this.tbTemporada.BackColor = System.Drawing.Color.Bisque;
            this.tbTemporada.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbTemporada.Controls.Add(this.label8);
            this.tbTemporada.Controls.Add(this.label6);
            this.tbTemporada.Controls.Add(this.btnBuscarCarpeta);
            this.tbTemporada.Controls.Add(this.lblCarpeta);
            this.tbTemporada.Controls.Add(this.lstTemporadas);
            this.tbTemporada.Controls.Add(this.label7);
            this.tbTemporada.Controls.Add(this.groupBox3);
            this.tbTemporada.Controls.Add(this.groupBox2);
            this.tbTemporada.Controls.Add(this.label4);
            this.tbTemporada.Controls.Add(this.label5);
            this.tbTemporada.Controls.Add(this.label3);
            this.tbTemporada.Controls.Add(this.label2);
            this.tbTemporada.Controls.Add(this.btnIntrJorn);
            this.tbTemporada.Controls.Add(this.btnIntrTemp);
            this.tbTemporada.Controls.Add(this.btnVerArch);
            this.tbTemporada.Controls.Add(this.txtNombreArchBase);
            this.tbTemporada.Location = new System.Drawing.Point(4, 22);
            this.tbTemporada.Name = "tbTemporada";
            this.tbTemporada.Size = new System.Drawing.Size(558, 102);
            this.tbTemporada.TabIndex = 2;
            this.tbTemporada.Tag = "3";
            this.tbTemporada.Text = "Escrutinio contra jornadas";
            this.tbTemporada.ToolTipText = "Escruta fichero/s contra la correspondiente temporada y jornada";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(8, 72);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(49, 12);
            this.label8.TabIndex = 90;
            this.label8.Text = "Carpeta:";
            // 
            // label6
            // 
            this.label6.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(8, 8);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(114, 32);
            this.label6.TabIndex = 89;
            this.label6.Text = "Plantilla de nombre de archivo:";
            // 
            // btnBuscarCarpeta
            // 
            this.btnBuscarCarpeta.BackColor = System.Drawing.Color.LightSalmon;
            this.btnBuscarCarpeta.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnBuscarCarpeta.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnBuscarCarpeta.Image = ((System.Drawing.Image)(resources.GetObject("btnBuscarCarpeta.Image")));
            this.btnBuscarCarpeta.Location = new System.Drawing.Point(63, 70);
            this.btnBuscarCarpeta.Name = "btnBuscarCarpeta";
            this.btnBuscarCarpeta.Size = new System.Drawing.Size(24, 23);
            this.btnBuscarCarpeta.TabIndex = 88;
            this.btnBuscarCarpeta.UseVisualStyleBackColor = false;
            this.btnBuscarCarpeta.Click += new System.EventHandler(this.btnBuscarCarpeta_Click);
            // 
            // lblCarpeta
            // 
            this.lblCarpeta.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblCarpeta.Location = new System.Drawing.Point(88, 70);
            this.lblCarpeta.Name = "lblCarpeta";
            this.lblCarpeta.Size = new System.Drawing.Size(216, 23);
            this.lblCarpeta.TabIndex = 36;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.ForeColor = System.Drawing.Color.Black;
            this.label7.Location = new System.Drawing.Point(464, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(66, 13);
            this.label7.TabIndex = 33;
            this.label7.Text = "Temporadas";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.rj1);
            this.groupBox3.Controls.Add(this.rj2);
            this.groupBox3.Location = new System.Drawing.Point(318, 48);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(132, 40);
            this.groupBox3.TabIndex = 32;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Dígitos jornadas";
            // 
            // rj1
            // 
            this.rj1.Location = new System.Drawing.Point(76, 15);
            this.rj1.Name = "rj1";
            this.rj1.Size = new System.Drawing.Size(32, 16);
            this.rj1.TabIndex = 4;
            this.rj1.Text = "1";
            // 
            // rj2
            // 
            this.rj2.Checked = true;
            this.rj2.Location = new System.Drawing.Point(16, 16);
            this.rj2.Name = "rj2";
            this.rj2.Size = new System.Drawing.Size(32, 16);
            this.rj2.TabIndex = 3;
            this.rj2.TabStop = true;
            this.rj2.Text = "2";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.rt2);
            this.groupBox2.Controls.Add(this.rt4);
            this.groupBox2.Location = new System.Drawing.Point(318, 8);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(132, 40);
            this.groupBox2.TabIndex = 31;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Dígitos temporadas";
            // 
            // rt2
            // 
            this.rt2.Location = new System.Drawing.Point(76, 16);
            this.rt2.Name = "rt2";
            this.rt2.Size = new System.Drawing.Size(32, 16);
            this.rt2.TabIndex = 2;
            this.rt2.Text = "2";
            // 
            // rt4
            // 
            this.rt4.Checked = true;
            this.rt4.Location = new System.Drawing.Point(16, 16);
            this.rt4.Name = "rt4";
            this.rt4.Size = new System.Drawing.Size(32, 16);
            this.rt4.TabIndex = 1;
            this.rt4.TabStop = true;
            this.rt4.Text = "4";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.Color.Blue;
            this.label4.Location = new System.Drawing.Point(128, 48);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(13, 15);
            this.label4.TabIndex = 30;
            this.label4.Text = "/j";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.ForeColor = System.Drawing.Color.Blue;
            this.label5.Location = new System.Drawing.Point(144, 48);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(46, 14);
            this.label5.TabIndex = 29;
            this.label5.Text = "Jornada";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.Color.Blue;
            this.label3.Location = new System.Drawing.Point(128, 32);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(13, 15);
            this.label3.TabIndex = 28;
            this.label3.Text = "/t";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.Blue;
            this.label2.Location = new System.Drawing.Point(144, 32);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(61, 14);
            this.label2.TabIndex = 27;
            this.label2.Text = "Temporada";
            // 
            // txtNombreArchBase
            // 
            this.txtNombreArchBase.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtNombreArchBase.Location = new System.Drawing.Point(128, 8);
            this.txtNombreArchBase.Name = "txtNombreArchBase";
            this.txtNombreArchBase.Size = new System.Drawing.Size(184, 21);
            this.txtNombreArchBase.TabIndex = 22;
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.tabControl1);
            this.panel1.Location = new System.Drawing.Point(16, 144);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(568, 128);
            this.panel1.TabIndex = 55;
            // 
            // fd
            // 
            this.fd.Title = "Selecciona los archivos";
            // 
            // btnPosiblesPremios
            // 
            this.btnPosiblesPremios.BackColor = System.Drawing.Color.DarkSalmon;
            this.btnPosiblesPremios.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnPosiblesPremios.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnPosiblesPremios.Image = ((System.Drawing.Image)(resources.GetObject("btnPosiblesPremios.Image")));
            this.btnPosiblesPremios.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnPosiblesPremios.Location = new System.Drawing.Point(456, 280);
            this.btnPosiblesPremios.Name = "btnPosiblesPremios";
            this.btnPosiblesPremios.Size = new System.Drawing.Size(128, 32);
            this.btnPosiblesPremios.TabIndex = 57;
            this.btnPosiblesPremios.Text = "Posibles premios";
            this.btnPosiblesPremios.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnPosiblesPremios.UseVisualStyleBackColor = false;
            this.btnPosiblesPremios.Click += new System.EventHandler(this.btnPosiblesPremios_Click);
            // 
            // btnActualizaCG
            // 
            this.btnActualizaCG.BackColor = System.Drawing.Color.DarkSalmon;
            this.btnActualizaCG.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnActualizaCG.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnActualizaCG.Image = ((System.Drawing.Image)(resources.GetObject("btnActualizaCG.Image")));
            this.btnActualizaCG.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnActualizaCG.Location = new System.Drawing.Point(318, 56);
            this.btnActualizaCG.Name = "btnActualizaCG";
            this.btnActualizaCG.Size = new System.Drawing.Size(27, 21);
            this.btnActualizaCG.TabIndex = 59;
            this.btnActualizaCG.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.toolTip1.SetToolTip(this.btnActualizaCG, "Actualizar Columna Ganadora desde Free1X2.com");
            this.btnActualizaCG.UseVisualStyleBackColor = false;
            this.btnActualizaCG.Click += new System.EventHandler(this.btnActualizaCG_Click);
            // 
            // EscrutiniosFrm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.Bisque;
            this.ClientSize = new System.Drawing.Size(608, 612);
            this.ControlBox = false;
            this.Controls.Add(this.btnPosiblesPremios);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.chkVerPremiadas);
            this.Controls.Add(this.btnVerPremiadas);
            this.Controls.Add(this.listaFicheros);
            this.Controls.Add(this.btnGrabaCols);
            this.Controls.Add(this.btnEnableSel);
            this.Controls.Add(this.btnDisabSel);
            this.Controls.Add(this.txtNoAciertos);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.dgResultados);
            this.Controls.Add(this.lblTime);
            this.Controls.Add(this.btnFileOrig);
            this.Controls.Add(this.btnCancelar);
            this.Controls.Add(this.btnComienzo);
            this.Name = "EscrutiniosFrm";
            this.Text = "Escrutinios";
            this.Load += new System.EventHandler(this.EscrutiniosFrm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgResultados)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.tpSimple.ResumeLayout(false);
            this.tpSimple.PerformLayout();
            this.tbFichero.ResumeLayout(false);
            this.tbTemporada.ResumeLayout(false);
            this.tbTemporada.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		
		#endregion

		void BtnCancelarClick(object sender, EventArgs e)
		{
			if(escrutador != null)
			{
				escrutador.PararEscrutinio();
			}

			Close();
		}
		
		void BtnComienzoClick(object sender, EventArgs e)
		{
			if(btnComienzo.Text == "Escrutar!" && SonDatosValidos() ) 
			{
				btnComienzo.Text = "Parar escrutinio!";
				RealizaEscrutinio();
				btnComienzo.Text = "Escrutar!";
				btnEnableSel.Enabled = true;
				btnDisabSel.Enabled = true;
				
				if(chkVerPremiadas.Checked)
				{
					btnVerPremiadas.Enabled = true;
				}
				else
				{
					btnVerPremiadas.Enabled = false;
				}
			}
			else if(btnComienzo.Text == "Parar escrutinio!")
			{
				escrutador.PararEscrutinio();
			}
		}
								
		void BtnFileOrigClick(object sender, EventArgs e)
		{
			// Prepara el cuadro de diálogo.
			OpenFileDialog abreFiltroDialog = new OpenFileDialog();
			abreFiltroDialog.InitialDirectory = "Columnas\\" ;
			abreFiltroDialog.Filter = "Columnas(*.txt)|*.txt|Todos los archivos (*.*)|*.*" ;
			abreFiltroDialog.Multiselect=true;
			if(abreFiltroDialog.ShowDialog() == DialogResult.OK)
		    {
				// Elimina la lista de ficheros anteriores
				archivosComb=null;
				listaFicheros.Items.Clear();
				archivosComb=new string[abreFiltroDialog.FileNames.Length];
				for(int i=0;i<archivosComb.Length ;i++)
				{
					archivosComb[i]=abreFiltroDialog.FileNames[i];
					listaFicheros.Items.Add(Path.GetFileName(archivosComb[i]));
				}
		    }
		}

		void BtnFileRefClick(object sender, EventArgs e)
		{
			OpenFileDialog abreFiltroDialog = new OpenFileDialog();
			abreFiltroDialog.InitialDirectory = "Columnas\\" ;
			abreFiltroDialog.Filter = "Columnas(*.txt)|*.txt|Todos los archivos (*.*)|*.*" ;
			if(abreFiltroDialog.ShowDialog() == DialogResult.OK)
		    {
		    	archivoRef = abreFiltroDialog.FileName;		    	
		    	lblFileRef.Text = Path.GetFileName(archivoRef);
				lblFileRef.Tag = archivoRef;
		    }
		}

		private void DgResultados_Click(object sender, MouseEventArgs e)
		{	
			Point pt = new Point(e.X, e.Y);
			DataGrid.HitTestInfo hti = dgResultados.HitTest(pt);

			if(hti.Type == DataGrid.HitTestType.Cell)
			{
				dgResultados.CurrentCell = new DataGridCell(hti.Row, hti.Column);
				dgResultados.Select(hti.Row);	
			
				if((bool)dgResultados[hti.Row,0] == false)
				{
					dgResultados[hti.Row,0] = true;
				}
				else
				{
					dgResultados[hti.Row,0] = false;
				}
			}		
		}

		private void btnEnableSel_Click(object sender, EventArgs e)
		{
			PonerValorMarcadoGlobal( true );
		}

		private void btnDisabSel_Click(object sender, EventArgs e)
		{
			PonerValorMarcadoGlobal( false );
		} 

		protected void PonerValorMarcadoGlobal(bool seleccionado)
		{
			foreach(DataRow row in resultadosDS.Tables["Resultados"].Rows)
			{
				row["Seleccionado"] = seleccionado;	
			}		
		}

		private void btnGrabaCols_Click(object sender, EventArgs e)
		{
			SaveFileDialog saveDialog = new SaveFileDialog();
			saveDialog.InitialDirectory = "Columnas\\" ;
			saveDialog.Filter = "Columnas(*.txt)|*.txt|Todos los archivos (*.*)|*.*" ;
					
			if(saveDialog.ShowDialog() == DialogResult.OK)
			{	
				string nombreArchivoComb = saveDialog.FileName;
                IArchivoColumnas archivo = new ArchivoColumnasTexto(nombreArchivoComb);
				
				DataView dv = new DataView(resultadosDS.Tables["Resultados"]);
				dv.RowFilter = "Seleccionado = true";

				foreach(DataRowView rowView in dv)
				{
					archivo.GuardarCols( rowView["Columna"].ToString() );
				}          

				archivo.Cerrar();
			}
		}

		private void btnVerPremiadas_Click(object sender, EventArgs e)
		{
		    ColumnasPremiadasFrm form = new ColumnasPremiadasFrm();
			ColumnasPremiadas colPremiada;
		    ListViewItem.ListViewSubItem[] l = new ListViewItem.ListViewSubItem[5];

			for(int i=0;i<listaPremiadas.Count;i++)
			{
			    colPremiada=(ColumnasPremiadas)listaPremiadas[i];
				ListViewItem li = new ListViewItem(Path.GetFileName(colPremiada.Fichero));
				l[0]=new ListViewItem.ListViewSubItem(li,colPremiada.Jornada.ToString());
				l[1]=new ListViewItem.ListViewSubItem(li,colPremiada.Columna);
				l[2]=new ListViewItem.ListViewSubItem(li,colPremiada.Premio.ToString());
				l[3]=new ListViewItem.ListViewSubItem(li,colPremiada.NoColumna.ToString());
				int orden = colPremiada.NoColumna%8;
				if(orden==0) orden=8;
				string tmp = colPremiada.NoBoleto+" ("+orden+")";
				l[4]=new ListViewItem.ListViewSubItem(li,tmp);
				for(int j=0;j<l.Length;j++)
				{
					li.SubItems.Add(l[j]);
				}
				form.listaResumen.Items.Add(li);
			}

			if(form.listaResumen.Items.Count > 0)
			{
				form.listaResumen.EnsureVisible(0);				
			}
			form.ShowDialog();
		}

	    private void EscrutiniosFrm_Load(object sender, EventArgs e)
		{
			txtColGanadora.MaxLength=VariablesGlobales.NumeroPartidos;
			seleccionarTab();
		}

		private void partidoClicado(object sender, MouseEventArgs e)
		{
			string[] resultados={" ","1","X","2"};
			Label lblPartido=(Label)sender;
			int numPartido=Convert.ToInt16(lblPartido.Text);
			TextBox partido=buscarCuadroPartido(numPartido);
			string resultado=partido.Text;
			if(resultado.Length==0) resultado=" ";
			int actual=Array.IndexOf(resultados,resultado);
			if(e.Button==MouseButtons.Left)
			{
				actual++;
				if(actual>3) actual=0;
			}
			else if(e.Button==MouseButtons.Right )
			{
				actual--;
				if(actual<0) actual=3;
			}
			partido.Text=resultados[actual];
			resultadoCambiado(partido,EventArgs.Empty);
		}

	    private TextBox buscarCuadroPartido(int numPartido)
		{
		    int numControl=0;
		    for(int i = 0; i < tpSimple.Controls.Count; i++)
		    {
		        //usamos el "as" para convertir un obejto al tipo que queramos
				//Si el objeto es de ese tipo se convierte y se asigna a la
				//variable, si no, su valor sera null (Nothing in VB)
		        TextBox partido = tpSimple.Controls[i] as TextBox;
		        //si el objeto no esta vacio, tenemos el tipo de objeto que 
				//buscamos...
				if(partido!= null)
				{
					string nombreControl = partido.Name;
					try
					{
						numControl=Convert.ToInt16(nombreControl.Substring(5));
					}
					catch{}
					if(numControl==numPartido) return partido;
				}
		    }
		    return null;
		}

		private void resultadoCambiado(object sender, EventArgs e)
		{
			string[] resultados={" ","1","X","2","*"};
			TextBox partido=(TextBox)sender;
			string resultado=partido.Text;
			if(resultado.Length==0) return;
			int actual=Array.IndexOf(resultados,resultado);
			if(actual<0)
			{
				MessageBox.Show("Hay carácteres no válidos en el resultado.","Free1X2",MessageBoxButtons.OK,MessageBoxIcon.Error);
				partido.Focus();
				return;
			}
		    string col="";
		    for(int i=0;i<VariablesGlobales.NumeroPartidos;i++)
		    {
		        TextBox t = buscarCuadroPartido(i+1);
		        if(t.Text.Trim().Length>0)
					col+=t.Text;
				else
					col+="*";
		    }
		    txtColGanadora.Text=col;
		}

        private void columnaCambiada(object sender, EventArgs e)
        {
            ActualizarResultadosCasillas();
        }

	    private void ActualizarResultadosCasillas()
        {

            string[] resultados ={ " ", "1", "X", "2", "*", "S" };
            string col = txtColGanadora.Text;
            bool hayError = false;
            for (int i = 0; i < col.Length; i++)
            {
                string signo = col.Substring(i, 1);
                if (signo.Trim().Length == 0) signo = "*";
                int actual = Array.IndexOf(resultados, signo);
                if (actual < 0)
                {
                    hayError = true;
                    col.Replace(signo, "*");
                }
                else
                {
                    TextBox t = buscarCuadroPartido(i + 1);
                    t.Text = signo;
                    if (signo == "S")
                    {
                        t.ForeColor = Color.Red;
                    }
                    else
                    {
                        t.ForeColor = Color.Black;
                    }
                }
            }
            if (hayError) MessageBox.Show("Había carácteres no válidos en la columna que han sido eliminados.", "Free1X2", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }


		private void crearDataset()
		{
		    dsJornadas=new DataSet("Resultados");
			// Crea la estructura del dataset
			DataTable myDataTable = new DataTable("Resultados");
			//Temporada
			DataColumn myDataColumn = new DataColumn();
			myDataColumn.DataType = Type.GetType("System.Int16");
			myDataColumn.ColumnName = "Temp";
			myDataTable.Columns.Add(myDataColumn);
			// Jornada
			myDataColumn = new DataColumn();
			myDataColumn.DataType = Type.GetType("System.Int16");
			myDataColumn.ColumnName = "Jorn";
			myDataTable.Columns.Add(myDataColumn);
			//Quiniela
			myDataColumn = new DataColumn();
			myDataColumn.DataType = Type.GetType("System.String");
			myDataColumn.ColumnName = "Quiniela";
			myDataColumn.MaxLength=15;
			myDataTable.Columns.Add(myDataColumn);
			// Abre el fichero de texto con los resultados
			StreamReader sr = new StreamReader( Application.StartupPath + "/Jornadas/Resultados.txt" );
		    while( sr.Peek() >= 0 )
			{
				// Lee la linea y la almacena en una matriz
				string linea = sr.ReadLine();
				string[] resultado = linea.Split((char)9);
				// Añade filas
				DataRow myDataRow = myDataTable.NewRow();
				myDataRow["Temp"] = Convert.ToInt16(resultado[0]);
				myDataRow["Jorn"] = Convert.ToInt16(resultado[1]);
				myDataRow["Quiniela"] = resultado[2];
				myDataTable.Rows.Add(myDataRow);
			}
			dsJornadas.Tables.Add(myDataTable);
			int nr=myDataTable.Rows.Count;
			for(int i=Convert.ToInt16(myDataTable.Rows[nr-1]["Temp"]);i>=Convert.ToInt16(myDataTable.Rows[0]["Temp"]);i--)
			{
				lstTemporadas.Items.Add(i.ToString());
			}
		}

		private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
		{
			seleccionarTab();
		}

		private void seleccionarTab()
		{
			int i;
			// Quita el resalte de color a los tabs
			for(i=0;i<tabControl1.TabPages.Count;i++)
			{
				tabControl1.TabPages[i].ImageIndex=0;
			}
			i=tabControl1.SelectedIndex;
			tipoEscrutinio=Convert.ToInt16(tabControl1.TabPages[i].Tag);
			if(tipoEscrutinio==3)
			{
				btnFileOrig.Visible=false;
				listaFicheros.Visible=false;
			}
			else
			{
				btnFileOrig.Visible=true;
				listaFicheros.Visible=true;
			}
			tabControl1.TabPages[i].ImageIndex=1;
		}

		private void btnVerArch_Click(object sender, EventArgs e)
		{
			fd.Filter="";
			fd.FileNames.Initialize();
			fd.ShowDialog();
			if(fd.FileName.Length>0)
			{
				FileInfo fileInfo= new FileInfo(fd.FileName);
				txtNombreArchBase.Text=fileInfo.Name;
			}
		}

		private void btnBuscarCarpeta_Click(object sender, EventArgs e)
		{
			if(fd.FileName.Length==0)
				fbd.SelectedPath=Application.StartupPath+"/Columnas/";
			else
				fbd.SelectedPath=Path.GetDirectoryName(fd.FileName);
			DialogResult dr=fbd.ShowDialog();
			if(dr==DialogResult.OK)
				lblCarpeta.Text=fbd.SelectedPath;
		}

		private void incluirPrefijo_click(object sender, EventArgs e)
		{
			Button btn=(Button)sender;
			int i=txtNombreArchBase.SelectionStart;
			string cadIzq, cadDer="";
			if(txtNombreArchBase.Text.Length>(i+1))
			{
				cadIzq=txtNombreArchBase.Text.Substring(0,i);
				cadDer=txtNombreArchBase.Text.Substring(i);
			}
			else
				cadIzq=txtNombreArchBase.Text;
			txtNombreArchBase.Text=cadIzq+btn.Text+cadDer;
			txtNombreArchBase.Focus();
			txtNombreArchBase.SelectionStart=i+2;
			txtNombreArchBase.SelectionLength=0;
		}

		private void btnPosiblesPremios_Click(object sender, EventArgs e)
		{
			PosiblesPremiosFrm f=new PosiblesPremiosFrm();
			f.Show();
		}

        private void btnActualizaCG_Click(object sender, EventArgs e)
        {
            Free1X2WService free1X2WService = new Free1X2WService();
            JornadaActual jornadaActual = free1X2WService.ObtenerJornadaActual();

            StringBuilder sb = new StringBuilder();
            sb.Append(jornadaActual.P1);
            sb.Append(jornadaActual.P2);
            sb.Append(jornadaActual.P3);
            sb.Append(jornadaActual.P4);
            sb.Append(jornadaActual.P5);
            sb.Append(jornadaActual.P6);
            sb.Append(jornadaActual.P7);
            sb.Append(jornadaActual.P8);
            sb.Append(jornadaActual.P9);
            sb.Append(jornadaActual.P10);
            sb.Append(jornadaActual.P11);
            sb.Append(jornadaActual.P12);
            sb.Append(jornadaActual.P13);
            sb.Append(jornadaActual.P14);
            sb.Append(jornadaActual.P15);
            try
            {
                txtColGanadora.Text = sb.ToString().Substring(0, VariablesGlobales.NumeroPartidos);
            }
            catch (Exception)
            {
                txtColGanadora.Text = "****************".Substring(0, VariablesGlobales.NumeroPartidos);
            }

            ActualizarResultadosCasillas();

        }

	}
}
