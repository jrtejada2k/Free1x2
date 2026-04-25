// created on 17/01/2004 at 11:43
// Free1X2 : Programa de quinielas "libre"
// Copyright (C) 2006 Toni Moreno
// Copyright (C) xfsf
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
using System.IO;
using System.Collections;
using System.Windows.Forms;
using System.Data;
using System.Drawing;

using Free1X2;
using Free1X2.Escrutinio;
using Free1X2.Utils;
using Free1X2.EntradaSalida;

using Free1X2.UI.Modern.Theming;
namespace Free1X2.UI
{
	public class EscrutarCombinacionesFrm : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Button btnCancelar;
		private System.Windows.Forms.Button btnFileOrig;
		private System.Windows.Forms.DataGrid dgResultados;
		private System.Windows.Forms.Label lblTime;
		private System.Windows.Forms.Button btnComienzo;
		private System.Windows.Forms.Label lblFileRef;
		private System.Windows.Forms.Label labCG;
		private System.Windows.Forms.Button btnFileRef;
		
		private EscrutadorComb escrutador;
		private DataSet resultadosDS = null;
		private DataSet dsJornadas=null;
		
		private string[] archivosComb;
		private string archivoRef = "";
		private DateTime hora0, hora9;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button btnDisabSel;
		private System.Windows.Forms.Button btnEnableSel;
		private System.Windows.Forms.Button btnGrabaCols;
		private System.Windows.Forms.ListBox listaFicheros;
		private System.Windows.Forms.TextBox txtNoAciertos;
		private System.Windows.Forms.Button btnVerPremiadas;
		private System.Windows.Forms.CheckBox chkVerPremiadas;
		private System.Windows.Forms.TextBox txtCG1;
		private System.Windows.Forms.TextBox txtCG2;
		private System.Windows.Forms.TextBox txtCG3;
		private System.Windows.Forms.TextBox txtCG4;
		private System.Windows.Forms.TextBox txtCG5;
		private System.Windows.Forms.TextBox txtCG6;
		private System.Windows.Forms.TextBox txtCG7;
		private System.Windows.Forms.TextBox txtCG8;
		private System.Windows.Forms.TextBox txtCG9;
		private System.Windows.Forms.TextBox txtCG10;
		private System.Windows.Forms.TextBox txtCG11;
		private System.Windows.Forms.TextBox txtCG12;
		private System.Windows.Forms.TextBox txtCG13;
		private System.Windows.Forms.TextBox txtCG14;
		private System.Windows.Forms.Label lblCG1;
		private System.Windows.Forms.Label lblCG2;
		private System.Windows.Forms.Label lblCG3;
		private System.Windows.Forms.Label lblCG4;
		private System.Windows.Forms.Label lblCG5;
		private System.Windows.Forms.Label lblCG6;
		private System.Windows.Forms.Label lblCG7;
		private System.Windows.Forms.Label lblCG8;
		private System.Windows.Forms.Label lblCG9;
		private System.Windows.Forms.Label lblCG10;
		private System.Windows.Forms.Label lblCG11;
		private System.Windows.Forms.Label lblCG12;
		private System.Windows.Forms.Label lblCG13;
		private System.Windows.Forms.Label lblCG14;
		private System.Windows.Forms.TextBox txtColGanadora;
		private System.Windows.Forms.ToolTip toolTip1;
		private System.ComponentModel.IContainer components;
		private System.Windows.Forms.TabControl tabControl1;
		private System.Windows.Forms.TabPage tpSimple;
		private System.Windows.Forms.TabPage tbFichero;
		private System.Windows.Forms.TabPage tbTemporada;
		private ArrayList listaPremiadas=new ArrayList();
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.RadioButton rj1;
		private System.Windows.Forms.RadioButton rj2;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.RadioButton rt2;
		private System.Windows.Forms.RadioButton rt4;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Button btnIntrJorn;
		private System.Windows.Forms.Button btnIntrTemp;
		private System.Windows.Forms.Button btnVerArch;
		private System.Windows.Forms.TextBox txtNombreArchBase;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Panel panel1;
		private int tipoEscrutinio=0;
		private System.Windows.Forms.OpenFileDialog fd;
		private System.Windows.Forms.ListBox lstTemporadas;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.FolderBrowserDialog fbd;
		private System.Windows.Forms.Button btnBuscarCarpeta;
        private System.Windows.Forms.Label lblCarpeta;
		private System.Windows.Forms.Button btnPosiblesPremios;
		private ImageList imageList1=new ImageList();
		
		public EscrutarCombinacionesFrm()
		{
			InitializeComponent();
			InicializaGridResultados();
			crearDataset();
            FormulariosHelper fH = new FormulariosHelper();
            fH.Traducir(this);
		}
        protected override void OnLoad(System.EventArgs e)
        {
            base.OnLoad(e);
            ModernTheme.ApplyToForm(this);
        }


		protected void InicializaResultadosDataSet()
		{
			string nombre="";
			resultadosDS = new DataSet();

			DataTable newTable = new DataTable("Resultados");
			newTable.Columns.Add("Seleccionado", typeof(bool));
			newTable.Columns.Add("LineaID", typeof(int));
			newTable.Columns.Add("Columna", typeof(string));
			newTable.Columns.Add("Archivo", typeof(string));
			for(int i=0;i<=VariablesGlobales.NumeroPartidos;i++)
			{
				nombre="P"+i.ToString();
				newTable.Columns.Add(nombre, typeof(int));
			}
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
			DataGridTextBoxColumn cs = null;

			//Archivo
			cs = new DataGridTextBoxColumn();
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
				cs.MappingName = "P" + colAciertos[i-1].ToString();
				cs.HeaderText = colAciertos[i-1].ToString();
				cs.Width = 40;
				tableStyle.GridColumnStyles.Add(cs);			
			}
			tableStyle.AllowSorting=true;

			//primero borrar los TableStyles existentes
			dgResultados.TableStyles.Clear();
		    dgResultados.TableStyles.Add(tableStyle);	
			//dgResultados.MouseUp += new System.Windows.Forms.MouseEventHandler(DgResultados_Click);
		}		
		
		protected void GridDataBind()
		{
			dgResultados.SetDataBinding(resultadosDS, "Resultados");			
		}
		
		protected void RealizaEscrutinio()
		{
			/*
				1- Seleccionar la/s columna/s ganadora/s.
				2- Obtener todas las columnas con posibles premios dentro del rango
				3- Comprobar si las columnas cumplen las condiciones (si cumple, añadir a premios)
			*/

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
				if(rt4.Checked==true) dt=4; else dt=2;
				if(rj2.Checked==true) dj=2; else dj=1;
				ArrayList listaInicialArchivos=new ArrayList();
				int temporada=0, jornada=0, x=0;
				string temp="", jorn="";
				string consulta="";
				// Creamos un dataView para seleccionar las temporadas en el dataset
				for(int i=0;i<lstTemporadas.SelectedIndices.Count;i++)
				{
					x=lstTemporadas.SelectedIndices[i];
					temporada=Convert.ToInt16(lstTemporadas.Items[x]);
					consulta+=" or Temp="+temporada.ToString();
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
				string colGan, archivo;
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
						if(numeros.IndexOf(jorn.Substring(1))>=0)
							dj=2;
						else
						{
							dj=1;
							jorn=jorn.Substring(0,1);
						}
						jornada=Convert.ToInt16(jorn);
					}
					dv.RowFilter="Temp="+temporada.ToString()+" and Jorn="+jornada.ToString();
					colGan=dv[0]["Quiniela"].ToString();
					// Escrutinio
					escrutador = new EscrutadorComb(colAciertos);
					escrutador.ArchivoColumnas = archivo;
					escrutador.AñadirAGanadoras = chkVerPremiadas.Checked;
					escrutador.ObtenerPosiblesPremios(colGan, colAciertos);
					escrutador.EscrutarCombinacion(jornada);
					sumarPremios(ref premiosGlobales, escrutador.PremiosTotales);
				}
			}
			else
			{
				for(int i=0;i<archivosComb.Length;i++)
				{
					escrutador = new EscrutadorComb(colAciertos);
					escrutador.ArchivoColumnas = archivosComb[i];
					escrutador.AñadirAGanadoras = chkVerPremiadas.Checked;									
				
					if( tipoEscrutinio==1)
					{
						string colGan = ObtenColGanadora();
						escrutador.ObtenerPosiblesPremios(colGan, colAciertos);
						escrutador.EscrutarCombinacion(0);
						sumarPremios(ref premiosGlobales, escrutador.PremiosTotales);
					}
					else if(tipoEscrutinio==2)
					{
                        IArchivoColumnas arch = new ArchivoColumnasTexto(lblFileRef.Tag.ToString());
						string[] ganadoras=arch.LeerTodasCols(false);
						for(int jorn=1; jorn<=ganadoras.Length; jorn++)
						{
							escrutador.ObtenerPosiblesPremios(ganadoras[jorn-1], colAciertos);
							escrutador.EscrutarCombinacion(jorn);
							sumarPremios(ref premiosGlobales, escrutador.PremiosTotales);
						}
					}
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
			if(txtCG1.Text.Trim() != "")
			{
				col += txtCG1.Text.Trim();
			}
			else
			{
				col += "*";
			}

			if(txtCG2.Text.Trim() != "")
			{
				col += txtCG2.Text.Trim();
			}
			else
			{
				col += "*";
			}

			if(txtCG3.Text.Trim() != "")
			{
				col += txtCG3.Text.Trim();
			}
			else
			{
				col += "*";
			}

			if(txtCG4.Text.Trim() != "")
			{
				col += txtCG4.Text.Trim();
			}
			else
			{
				col += "*";
			}

			if(txtCG5.Text.Trim() != "")
			{
				col += txtCG5.Text.Trim();
			}
			else
			{
				col += "*";
			}

			if(txtCG6.Text.Trim() != "")
			{
				col += txtCG6.Text.Trim();
			}
			else
			{
				col += "*";
			}

			if(txtCG7.Text.Trim() != "")
			{
				col += txtCG7.Text.Trim();
			}
			else
			{
				col += "*";
			}

			if(txtCG8.Text.Trim() != "")
			{
				col += txtCG8.Text.Trim();
			}
			else
			{
				col += "*";
			}

			if(txtCG9.Text.Trim() != "")
			{
				col += txtCG9.Text.Trim();
			}
			else
			{
				col += "*";
			}

			if(txtCG10.Text.Trim() != "")
			{
				col += txtCG10.Text.Trim();
			}
			else
			{
				col += "*";
			}

			if(txtCG11.Text.Trim() != "")
			{
				col += txtCG11.Text.Trim();
			}
			else
			{
				col += "*";
			}

			if(txtCG12.Text.Trim() != "")
			{
				col += txtCG12.Text.Trim();
			}
			else
			{
				col += "*";
			}

			if(txtCG13.Text.Trim() != "")
			{
				col += txtCG13.Text.Trim();
			}
			else
			{
				col += "*";
			}

			if(txtCG14.Text.Trim() != "")
			{
				col += txtCG14.Text.Trim();
			}
			else
			{
				col += "*";
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
					if(ch != 'x' && ch != 'X' && ch != '1' && ch != '2' && ch != '*') {
						msg = "La C.G. solo puede contener los caracteres: 1,X,2,*";
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EscrutarCombinacionesFrm));
            this.btnFileRef = new System.Windows.Forms.Button();
            this.labCG = new System.Windows.Forms.Label();
            this.lblFileRef = new System.Windows.Forms.Label();
            this.btnComienzo = new System.Windows.Forms.Button();
            this.lblTime = new System.Windows.Forms.Label();
            this.dgResultados = new System.Windows.Forms.DataGrid();
            this.btnFileOrig = new System.Windows.Forms.Button();
            this.btnCancelar = new System.Windows.Forms.Button();
            this.txtCG1 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtNoAciertos = new System.Windows.Forms.TextBox();
            this.btnDisabSel = new System.Windows.Forms.Button();
            this.btnEnableSel = new System.Windows.Forms.Button();
            this.btnGrabaCols = new System.Windows.Forms.Button();
            this.listaFicheros = new System.Windows.Forms.ListBox();
            this.btnVerPremiadas = new System.Windows.Forms.Button();
            this.chkVerPremiadas = new System.Windows.Forms.CheckBox();
            this.txtCG2 = new System.Windows.Forms.TextBox();
            this.txtCG3 = new System.Windows.Forms.TextBox();
            this.txtCG4 = new System.Windows.Forms.TextBox();
            this.txtCG5 = new System.Windows.Forms.TextBox();
            this.txtCG6 = new System.Windows.Forms.TextBox();
            this.txtCG7 = new System.Windows.Forms.TextBox();
            this.txtCG8 = new System.Windows.Forms.TextBox();
            this.txtCG9 = new System.Windows.Forms.TextBox();
            this.txtCG10 = new System.Windows.Forms.TextBox();
            this.txtCG11 = new System.Windows.Forms.TextBox();
            this.txtCG12 = new System.Windows.Forms.TextBox();
            this.txtCG13 = new System.Windows.Forms.TextBox();
            this.txtCG14 = new System.Windows.Forms.TextBox();
            this.lblCG1 = new System.Windows.Forms.Label();
            this.lblCG2 = new System.Windows.Forms.Label();
            this.lblCG3 = new System.Windows.Forms.Label();
            this.lblCG4 = new System.Windows.Forms.Label();
            this.lblCG5 = new System.Windows.Forms.Label();
            this.lblCG6 = new System.Windows.Forms.Label();
            this.lblCG7 = new System.Windows.Forms.Label();
            this.lblCG8 = new System.Windows.Forms.Label();
            this.lblCG9 = new System.Windows.Forms.Label();
            this.lblCG10 = new System.Windows.Forms.Label();
            this.lblCG11 = new System.Windows.Forms.Label();
            this.lblCG12 = new System.Windows.Forms.Label();
            this.lblCG13 = new System.Windows.Forms.Label();
            this.lblCG14 = new System.Windows.Forms.Label();
            this.txtColGanadora = new System.Windows.Forms.TextBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.btnIntrJorn = new System.Windows.Forms.Button();
            this.btnIntrTemp = new System.Windows.Forms.Button();
            this.btnVerArch = new System.Windows.Forms.Button();
            this.lstTemporadas = new System.Windows.Forms.ListBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tpSimple = new System.Windows.Forms.TabPage();
            this.tbFichero = new System.Windows.Forms.TabPage();
            this.tbTemporada = new System.Windows.Forms.TabPage();
            this.btnBuscarCarpeta = new System.Windows.Forms.Button();
            this.lblCarpeta = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
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
            this.label6 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.fd = new System.Windows.Forms.OpenFileDialog();
            this.fbd = new System.Windows.Forms.FolderBrowserDialog();
            this.btnPosiblesPremios = new System.Windows.Forms.Button();
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
            this.btnFileRef.Size = new System.Drawing.Size(138, 32);
            this.btnFileRef.TabIndex = 9;
            this.btnFileRef.Text = "Fichero referencia";
            this.btnFileRef.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnFileRef.UseVisualStyleBackColor = false;
            this.btnFileRef.Click += new System.EventHandler(this.BtnFileRefClick);
            // 
            // labCG
            // 
            this.labCG.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labCG.Location = new System.Drawing.Point(8, 24);
            this.labCG.Name = "labCG";
            this.labCG.Size = new System.Drawing.Size(120, 16);
            this.labCG.TabIndex = 7;
            this.labCG.Text = "Columna Ganadora :";
            // 
            // lblFileRef
            // 
            this.lblFileRef.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFileRef.Location = new System.Drawing.Point(162, 24);
            this.lblFileRef.Name = "lblFileRef";
            this.lblFileRef.Size = new System.Drawing.Size(334, 16);
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
            this.lblTime.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.lblTime.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTime.Location = new System.Drawing.Point(160, 280);
            this.lblTime.Name = "lblTime";
            this.lblTime.Size = new System.Drawing.Size(228, 32);
            this.lblTime.TabIndex = 12;
            this.lblTime.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblTime.Click += new System.EventHandler(this.LblTimeClick);
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
            this.dgResultados.Size = new System.Drawing.Size(568, 304);
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
            this.btnCancelar.Location = new System.Drawing.Point(496, 632);
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.Size = new System.Drawing.Size(88, 24);
            this.btnCancelar.TabIndex = 1;
            this.btnCancelar.Text = "Cancelar";
            this.btnCancelar.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnCancelar.UseVisualStyleBackColor = false;
            this.btnCancelar.Click += new System.EventHandler(this.BtnCancelarClick);
            // 
            // txtCG1
            // 
            this.txtCG1.BackColor = System.Drawing.Color.SeaShell;
            this.txtCG1.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtCG1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtCG1.Location = new System.Drawing.Point(144, 24);
            this.txtCG1.MaxLength = 1;
            this.txtCG1.Name = "txtCG1";
            this.txtCG1.Size = new System.Drawing.Size(16, 21);
            this.txtCG1.TabIndex = 22;
            this.toolTip1.SetToolTip(this.txtCG1, "Resultado de partidos");
            this.txtCG1.Leave += new System.EventHandler(this.resultadoCambiado);
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(30, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(84, 21);
            this.label1.TabIndex = 15;
            this.label1.Text = "No Aciertos:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // txtNoAciertos
            // 
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
            this.btnDisabSel.Location = new System.Drawing.Point(143, 632);
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
            this.btnEnableSel.Location = new System.Drawing.Point(16, 632);
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
            this.btnGrabaCols.Location = new System.Drawing.Point(284, 632);
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
            this.listaFicheros.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listaFicheros.Items.AddRange(new object[] {
            "(selecciona)"});
            this.listaFicheros.Location = new System.Drawing.Point(160, 40);
            this.listaFicheros.Name = "listaFicheros";
            this.listaFicheros.ScrollAlwaysVisible = true;
            this.listaFicheros.SelectionMode = System.Windows.Forms.SelectionMode.None;
            this.listaFicheros.Size = new System.Drawing.Size(328, 95);
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
            this.btnVerPremiadas.Location = new System.Drawing.Point(389, 632);
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
            this.chkVerPremiadas.Location = new System.Drawing.Point(206, 6);
            this.chkVerPremiadas.Name = "chkVerPremiadas";
            this.chkVerPremiadas.Size = new System.Drawing.Size(172, 24);
            this.chkVerPremiadas.TabIndex = 21;
            this.chkVerPremiadas.Text = "Activar Ver Premiadas";
            // 
            // txtCG2
            // 
            this.txtCG2.BackColor = System.Drawing.Color.SeaShell;
            this.txtCG2.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtCG2.Location = new System.Drawing.Point(161, 24);
            this.txtCG2.MaxLength = 1;
            this.txtCG2.Name = "txtCG2";
            this.txtCG2.Size = new System.Drawing.Size(16, 21);
            this.txtCG2.TabIndex = 23;
            this.toolTip1.SetToolTip(this.txtCG2, "Resultado de partidos");
            this.txtCG2.Leave += new System.EventHandler(this.resultadoCambiado);
            // 
            // txtCG3
            // 
            this.txtCG3.BackColor = System.Drawing.Color.SeaShell;
            this.txtCG3.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtCG3.Location = new System.Drawing.Point(178, 24);
            this.txtCG3.MaxLength = 1;
            this.txtCG3.Name = "txtCG3";
            this.txtCG3.Size = new System.Drawing.Size(16, 21);
            this.txtCG3.TabIndex = 24;
            this.toolTip1.SetToolTip(this.txtCG3, "Resultado de partidos");
            this.txtCG3.Leave += new System.EventHandler(this.resultadoCambiado);
            // 
            // txtCG4
            // 
            this.txtCG4.BackColor = System.Drawing.Color.SeaShell;
            this.txtCG4.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtCG4.Location = new System.Drawing.Point(195, 24);
            this.txtCG4.MaxLength = 1;
            this.txtCG4.Name = "txtCG4";
            this.txtCG4.Size = new System.Drawing.Size(16, 21);
            this.txtCG4.TabIndex = 25;
            this.toolTip1.SetToolTip(this.txtCG4, "Resultado de partidos");
            this.txtCG4.Leave += new System.EventHandler(this.resultadoCambiado);
            // 
            // txtCG5
            // 
            this.txtCG5.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.txtCG5.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtCG5.Location = new System.Drawing.Point(212, 24);
            this.txtCG5.MaxLength = 1;
            this.txtCG5.Name = "txtCG5";
            this.txtCG5.Size = new System.Drawing.Size(16, 21);
            this.txtCG5.TabIndex = 26;
            this.toolTip1.SetToolTip(this.txtCG5, "Resultado de partidos");
            this.txtCG5.Leave += new System.EventHandler(this.resultadoCambiado);
            // 
            // txtCG6
            // 
            this.txtCG6.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.txtCG6.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtCG6.Location = new System.Drawing.Point(229, 24);
            this.txtCG6.MaxLength = 1;
            this.txtCG6.Name = "txtCG6";
            this.txtCG6.Size = new System.Drawing.Size(16, 21);
            this.txtCG6.TabIndex = 27;
            this.toolTip1.SetToolTip(this.txtCG6, "Resultado de partidos");
            this.txtCG6.Leave += new System.EventHandler(this.resultadoCambiado);
            // 
            // txtCG7
            // 
            this.txtCG7.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.txtCG7.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtCG7.Location = new System.Drawing.Point(246, 24);
            this.txtCG7.MaxLength = 1;
            this.txtCG7.Name = "txtCG7";
            this.txtCG7.Size = new System.Drawing.Size(16, 21);
            this.txtCG7.TabIndex = 28;
            this.toolTip1.SetToolTip(this.txtCG7, "Resultado de partidos");
            this.txtCG7.Leave += new System.EventHandler(this.resultadoCambiado);
            // 
            // txtCG8
            // 
            this.txtCG8.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.txtCG8.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtCG8.Location = new System.Drawing.Point(263, 24);
            this.txtCG8.MaxLength = 1;
            this.txtCG8.Name = "txtCG8";
            this.txtCG8.Size = new System.Drawing.Size(16, 21);
            this.txtCG8.TabIndex = 29;
            this.toolTip1.SetToolTip(this.txtCG8, "Resultado de partidos");
            this.txtCG8.Leave += new System.EventHandler(this.resultadoCambiado);
            // 
            // txtCG9
            // 
            this.txtCG9.BackColor = System.Drawing.Color.SeaShell;
            this.txtCG9.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtCG9.Location = new System.Drawing.Point(280, 24);
            this.txtCG9.MaxLength = 1;
            this.txtCG9.Name = "txtCG9";
            this.txtCG9.Size = new System.Drawing.Size(16, 21);
            this.txtCG9.TabIndex = 30;
            this.toolTip1.SetToolTip(this.txtCG9, "Resultado de partidos");
            this.txtCG9.Leave += new System.EventHandler(this.resultadoCambiado);
            // 
            // txtCG10
            // 
            this.txtCG10.BackColor = System.Drawing.Color.SeaShell;
            this.txtCG10.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtCG10.Location = new System.Drawing.Point(297, 24);
            this.txtCG10.MaxLength = 1;
            this.txtCG10.Name = "txtCG10";
            this.txtCG10.Size = new System.Drawing.Size(16, 21);
            this.txtCG10.TabIndex = 31;
            this.toolTip1.SetToolTip(this.txtCG10, "Resultado de partidos");
            this.txtCG10.Leave += new System.EventHandler(this.resultadoCambiado);
            // 
            // txtCG11
            // 
            this.txtCG11.BackColor = System.Drawing.Color.SeaShell;
            this.txtCG11.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtCG11.Location = new System.Drawing.Point(314, 24);
            this.txtCG11.MaxLength = 1;
            this.txtCG11.Name = "txtCG11";
            this.txtCG11.Size = new System.Drawing.Size(16, 21);
            this.txtCG11.TabIndex = 32;
            this.toolTip1.SetToolTip(this.txtCG11, "Resultado de partidos");
            this.txtCG11.Leave += new System.EventHandler(this.resultadoCambiado);
            // 
            // txtCG12
            // 
            this.txtCG12.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.txtCG12.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtCG12.Location = new System.Drawing.Point(331, 24);
            this.txtCG12.MaxLength = 1;
            this.txtCG12.Name = "txtCG12";
            this.txtCG12.Size = new System.Drawing.Size(16, 21);
            this.txtCG12.TabIndex = 33;
            this.toolTip1.SetToolTip(this.txtCG12, "Resultado de partidos");
            this.txtCG12.Leave += new System.EventHandler(this.resultadoCambiado);
            // 
            // txtCG13
            // 
            this.txtCG13.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.txtCG13.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtCG13.Location = new System.Drawing.Point(348, 24);
            this.txtCG13.MaxLength = 1;
            this.txtCG13.Name = "txtCG13";
            this.txtCG13.Size = new System.Drawing.Size(16, 21);
            this.txtCG13.TabIndex = 34;
            this.toolTip1.SetToolTip(this.txtCG13, "Resultado de partidos");
            this.txtCG13.Leave += new System.EventHandler(this.resultadoCambiado);
            // 
            // txtCG14
            // 
            this.txtCG14.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.txtCG14.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtCG14.Location = new System.Drawing.Point(365, 24);
            this.txtCG14.MaxLength = 1;
            this.txtCG14.Name = "txtCG14";
            this.txtCG14.Size = new System.Drawing.Size(16, 21);
            this.txtCG14.TabIndex = 35;
            this.toolTip1.SetToolTip(this.txtCG14, "Resultado de partidos");
            this.txtCG14.Leave += new System.EventHandler(this.resultadoCambiado);
            // 
            // lblCG1
            // 
            this.lblCG1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCG1.Location = new System.Drawing.Point(144, 8);
            this.lblCG1.Name = "lblCG1";
            this.lblCG1.Size = new System.Drawing.Size(17, 16);
            this.lblCG1.TabIndex = 36;
            this.lblCG1.Text = "1";
            this.lblCG1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblCG1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.partidoClicado);
            // 
            // lblCG2
            // 
            this.lblCG2.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCG2.Location = new System.Drawing.Point(161, 8);
            this.lblCG2.Name = "lblCG2";
            this.lblCG2.Size = new System.Drawing.Size(17, 16);
            this.lblCG2.TabIndex = 37;
            this.lblCG2.Text = "2";
            this.lblCG2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblCG2.MouseUp += new System.Windows.Forms.MouseEventHandler(this.partidoClicado);
            // 
            // lblCG3
            // 
            this.lblCG3.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCG3.Location = new System.Drawing.Point(178, 8);
            this.lblCG3.Name = "lblCG3";
            this.lblCG3.Size = new System.Drawing.Size(17, 16);
            this.lblCG3.TabIndex = 38;
            this.lblCG3.Text = "3";
            this.lblCG3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblCG3.MouseUp += new System.Windows.Forms.MouseEventHandler(this.partidoClicado);
            // 
            // lblCG4
            // 
            this.lblCG4.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCG4.Location = new System.Drawing.Point(195, 8);
            this.lblCG4.Name = "lblCG4";
            this.lblCG4.Size = new System.Drawing.Size(17, 16);
            this.lblCG4.TabIndex = 39;
            this.lblCG4.Text = "4";
            this.lblCG4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblCG4.MouseUp += new System.Windows.Forms.MouseEventHandler(this.partidoClicado);
            // 
            // lblCG5
            // 
            this.lblCG5.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCG5.ForeColor = System.Drawing.Color.Maroon;
            this.lblCG5.Location = new System.Drawing.Point(212, 8);
            this.lblCG5.Name = "lblCG5";
            this.lblCG5.Size = new System.Drawing.Size(17, 16);
            this.lblCG5.TabIndex = 40;
            this.lblCG5.Text = "5";
            this.lblCG5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblCG5.MouseUp += new System.Windows.Forms.MouseEventHandler(this.partidoClicado);
            // 
            // lblCG6
            // 
            this.lblCG6.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCG6.ForeColor = System.Drawing.Color.Maroon;
            this.lblCG6.Location = new System.Drawing.Point(229, 8);
            this.lblCG6.Name = "lblCG6";
            this.lblCG6.Size = new System.Drawing.Size(17, 16);
            this.lblCG6.TabIndex = 41;
            this.lblCG6.Text = "6";
            this.lblCG6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblCG6.MouseUp += new System.Windows.Forms.MouseEventHandler(this.partidoClicado);
            // 
            // lblCG7
            // 
            this.lblCG7.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCG7.ForeColor = System.Drawing.Color.Maroon;
            this.lblCG7.Location = new System.Drawing.Point(246, 8);
            this.lblCG7.Name = "lblCG7";
            this.lblCG7.Size = new System.Drawing.Size(17, 16);
            this.lblCG7.TabIndex = 42;
            this.lblCG7.Text = "7";
            this.lblCG7.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblCG7.MouseUp += new System.Windows.Forms.MouseEventHandler(this.partidoClicado);
            // 
            // lblCG8
            // 
            this.lblCG8.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCG8.ForeColor = System.Drawing.Color.Maroon;
            this.lblCG8.Location = new System.Drawing.Point(263, 8);
            this.lblCG8.Name = "lblCG8";
            this.lblCG8.Size = new System.Drawing.Size(17, 16);
            this.lblCG8.TabIndex = 43;
            this.lblCG8.Text = "8";
            this.lblCG8.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblCG8.MouseUp += new System.Windows.Forms.MouseEventHandler(this.partidoClicado);
            // 
            // lblCG9
            // 
            this.lblCG9.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCG9.Location = new System.Drawing.Point(280, 8);
            this.lblCG9.Name = "lblCG9";
            this.lblCG9.Size = new System.Drawing.Size(17, 16);
            this.lblCG9.TabIndex = 44;
            this.lblCG9.Text = "9";
            this.lblCG9.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblCG9.MouseUp += new System.Windows.Forms.MouseEventHandler(this.partidoClicado);
            // 
            // lblCG10
            // 
            this.lblCG10.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCG10.Location = new System.Drawing.Point(297, 8);
            this.lblCG10.Name = "lblCG10";
            this.lblCG10.Size = new System.Drawing.Size(17, 16);
            this.lblCG10.TabIndex = 45;
            this.lblCG10.Text = "10";
            this.lblCG10.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblCG10.MouseUp += new System.Windows.Forms.MouseEventHandler(this.partidoClicado);
            // 
            // lblCG11
            // 
            this.lblCG11.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCG11.Location = new System.Drawing.Point(314, 8);
            this.lblCG11.Name = "lblCG11";
            this.lblCG11.Size = new System.Drawing.Size(17, 16);
            this.lblCG11.TabIndex = 46;
            this.lblCG11.Text = "11";
            this.lblCG11.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblCG11.MouseUp += new System.Windows.Forms.MouseEventHandler(this.partidoClicado);
            // 
            // lblCG12
            // 
            this.lblCG12.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCG12.ForeColor = System.Drawing.Color.Maroon;
            this.lblCG12.Location = new System.Drawing.Point(331, 8);
            this.lblCG12.Name = "lblCG12";
            this.lblCG12.Size = new System.Drawing.Size(17, 16);
            this.lblCG12.TabIndex = 47;
            this.lblCG12.Text = "12";
            this.lblCG12.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblCG12.MouseUp += new System.Windows.Forms.MouseEventHandler(this.partidoClicado);
            // 
            // lblCG13
            // 
            this.lblCG13.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCG13.ForeColor = System.Drawing.Color.Maroon;
            this.lblCG13.Location = new System.Drawing.Point(348, 8);
            this.lblCG13.Name = "lblCG13";
            this.lblCG13.Size = new System.Drawing.Size(17, 16);
            this.lblCG13.TabIndex = 48;
            this.lblCG13.Text = "13";
            this.lblCG13.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblCG13.MouseUp += new System.Windows.Forms.MouseEventHandler(this.partidoClicado);
            // 
            // lblCG14
            // 
            this.lblCG14.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCG14.ForeColor = System.Drawing.Color.Maroon;
            this.lblCG14.Location = new System.Drawing.Point(365, 8);
            this.lblCG14.Name = "lblCG14";
            this.lblCG14.Size = new System.Drawing.Size(17, 16);
            this.lblCG14.TabIndex = 49;
            this.lblCG14.Text = "14";
            this.lblCG14.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblCG14.MouseUp += new System.Windows.Forms.MouseEventHandler(this.partidoClicado);
            // 
            // txtColGanadora
            // 
            this.txtColGanadora.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtColGanadora.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
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
            this.lstTemporadas.Location = new System.Drawing.Point(456, 16);
            this.lstTemporadas.Name = "lstTemporadas";
            this.lstTemporadas.ScrollAlwaysVisible = true;
            this.lstTemporadas.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lstTemporadas.Size = new System.Drawing.Size(88, 82);
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
            this.tpSimple.Controls.Add(this.txtCG4);
            this.tpSimple.Controls.Add(this.txtCG10);
            this.tpSimple.Controls.Add(this.txtCG5);
            this.tpSimple.Controls.Add(this.txtCG12);
            this.tpSimple.Controls.Add(this.txtCG7);
            this.tpSimple.Controls.Add(this.labCG);
            this.tpSimple.Controls.Add(this.txtCG8);
            this.tpSimple.Controls.Add(this.txtCG9);
            this.tpSimple.Controls.Add(this.txtColGanadora);
            this.tpSimple.Controls.Add(this.lblCG14);
            this.tpSimple.Controls.Add(this.lblCG13);
            this.tpSimple.Controls.Add(this.lblCG12);
            this.tpSimple.Controls.Add(this.txtCG11);
            this.tpSimple.Controls.Add(this.lblCG11);
            this.tpSimple.Controls.Add(this.lblCG10);
            this.tpSimple.Controls.Add(this.lblCG9);
            this.tpSimple.Controls.Add(this.txtCG13);
            this.tpSimple.Controls.Add(this.txtCG14);
            this.tpSimple.Controls.Add(this.txtCG1);
            this.tpSimple.Controls.Add(this.lblCG1);
            this.tpSimple.Controls.Add(this.lblCG2);
            this.tpSimple.Controls.Add(this.lblCG3);
            this.tpSimple.Controls.Add(this.lblCG4);
            this.tpSimple.Controls.Add(this.lblCG5);
            this.tpSimple.Controls.Add(this.lblCG6);
            this.tpSimple.Controls.Add(this.lblCG7);
            this.tpSimple.Controls.Add(this.txtCG6);
            this.tpSimple.Controls.Add(this.lblCG8);
            this.tpSimple.Controls.Add(this.txtCG2);
            this.tpSimple.Controls.Add(this.txtCG3);
            this.tpSimple.Location = new System.Drawing.Point(4, 22);
            this.tpSimple.Name = "tpSimple";
            this.tpSimple.Size = new System.Drawing.Size(558, 102);
            this.tpSimple.TabIndex = 0;
            this.tpSimple.Tag = "1";
            this.tpSimple.Text = "Escrutinio simple";
            this.tpSimple.ToolTipText = "Escruta fichero/s contra una columna";
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
            this.tbTemporada.Controls.Add(this.btnBuscarCarpeta);
            this.tbTemporada.Controls.Add(this.lblCarpeta);
            this.tbTemporada.Controls.Add(this.label8);
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
            this.tbTemporada.Controls.Add(this.label6);
            this.tbTemporada.Location = new System.Drawing.Point(4, 22);
            this.tbTemporada.Name = "tbTemporada";
            this.tbTemporada.Size = new System.Drawing.Size(558, 102);
            this.tbTemporada.TabIndex = 2;
            this.tbTemporada.Tag = "3";
            this.tbTemporada.Text = "Escrutinio contra jornadas";
            this.tbTemporada.ToolTipText = "Escruta fichero/s contra la correspondiente temporada y jornada";
            // 
            // btnBuscarCarpeta
            // 
            this.btnBuscarCarpeta.BackColor = System.Drawing.Color.LightSalmon;
            this.btnBuscarCarpeta.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnBuscarCarpeta.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
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
            this.lblCarpeta.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblCarpeta.Location = new System.Drawing.Point(88, 70);
            this.lblCarpeta.Name = "lblCarpeta";
            this.lblCarpeta.Size = new System.Drawing.Size(216, 23);
            this.lblCarpeta.TabIndex = 36;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(8, 72);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(49, 12);
            this.label8.TabIndex = 35;
            this.label8.Text = "Carpeta:";
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
            this.groupBox3.Size = new System.Drawing.Size(130, 40);
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
            this.txtNombreArchBase.Location = new System.Drawing.Point(128, 8);
            this.txtNombreArchBase.Name = "txtNombreArchBase";
            this.txtNombreArchBase.Size = new System.Drawing.Size(184, 21);
            this.txtNombreArchBase.TabIndex = 22;
            // 
            // label6
            // 
            this.label6.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(8, 8);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(114, 32);
            this.label6.TabIndex = 23;
            this.label6.Text = "Plantilla de nombre de archivo:";
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
            // EscrutarCombinacionesFrm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.Bisque;
            this.ClientSize = new System.Drawing.Size(608, 694);
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
            this.Name = "EscrutarCombinacionesFrm";
            this.Text = "Escrutar combinaciones";
            this.Load += new System.EventHandler(this.EscrutiniosCombinacionesFrm_Load);
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

		void BtnCancelarClick(object sender, System.EventArgs e)
		{
			if(escrutador != null)
			{
				escrutador.PararEscrutinio();
			}

			this.Close();
		}
		
		void BtnComienzoClick(object sender, System.EventArgs e)
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
								
		void BtnFileOrigClick(object sender, System.EventArgs e)
		{
			// Prepara el cuadro de diálogo.
			OpenFileDialog abreFiltroDialog = new OpenFileDialog();
			abreFiltroDialog.InitialDirectory = "Columnas\\" ;
			abreFiltroDialog.Filter = "Todas las combinaciones(*.comb, *.xml)|*.comb; *.xml|Combinaciones(*.comb)|*.comb|Combinaciones(*.xml)|*.xml|Todos los archivos (*.*)|*.*" ;
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

		void BtnFileRefClick(object sender, System.EventArgs e)
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
		
		void RBtnCGCheckedChanged(object sender, System.EventArgs e)
		{
		}

		void RBtnFGCheckedChanged(object sender, System.EventArgs e)
		{
		}

		void LblTimeClick(object sender, System.EventArgs e)
		{
			
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

		private void btnEnableSel_Click(object sender, System.EventArgs e)
		{
			PonerValorMarcadoGlobal( true );
		}

		private void btnDisabSel_Click(object sender, System.EventArgs e)
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

		private void btnGrabaCols_Click(object sender, System.EventArgs e)
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


		private void btnVerPremiadas_Click(object sender, System.EventArgs e)
		{
			string tmp;
			int orden;
			ColumnasPremiadasFrm form = new ColumnasPremiadasFrm();
			ColumnasPremiadas colPremiada;
			ListViewItem li = null;
			ListViewItem.ListViewSubItem[] l = new ListViewItem.ListViewSubItem[5];

			for(int i=0;i<listaPremiadas.Count;i++)
			{
				colPremiada=new ColumnasPremiadas();
				colPremiada=(ColumnasPremiadas)listaPremiadas[i];
				li = new ListViewItem(Path.GetFileName(colPremiada.Fichero));
				l[0]=new System.Windows.Forms.ListViewItem.ListViewSubItem(li,colPremiada.Jornada.ToString());
				l[1]=new System.Windows.Forms.ListViewItem.ListViewSubItem(li,colPremiada.Columna);
				l[2]=new System.Windows.Forms.ListViewItem.ListViewSubItem(li,colPremiada.Premio.ToString());
				l[3]=new System.Windows.Forms.ListViewItem.ListViewSubItem(li,colPremiada.NoColumna.ToString());
				orden=colPremiada.NoColumna%8;
				if(orden==0) orden=8;
				tmp=colPremiada.NoBoleto+" ("+orden.ToString()+")";
				l[4]=new System.Windows.Forms.ListViewItem.ListViewSubItem(li,tmp);
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

		private void EscrutiniosCombinacionesFrm_Load(object sender, System.EventArgs e)
		{
			txtColGanadora.MaxLength=VariablesGlobales.NumeroPartidos;
			seleccionarTab();
		}

		private void partidoClicado(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			string[] resultados={" ","1","X","2"};
			Label lblPartido=(Label)sender;
			int numPartido=Convert.ToInt16(lblPartido.Text);
			TextBox partido=buscarCuadroPartido(numPartido);
			string resultado=partido.Text;
			if(resultado.Length==0) resultado=" ";
			int actual=Array.IndexOf(resultados,resultado);
			if(e.Button==System.Windows.Forms.MouseButtons.Left)
			{
				actual++;
				if(actual>3) actual=0;
			}
			else if(e.Button==System.Windows.Forms.MouseButtons.Right )
			{
				actual--;
				if(actual<0) actual=3;
			}
			partido.Text=resultados[actual];
			resultadoCambiado(partido,EventArgs.Empty);
		}

		private int buscarNumPartido(TextBox txt)
		{
			int numPartido=0;
			string nombreControl=txt.Name;
			try
			{
				numPartido=Convert.ToInt16(nombreControl.Substring(5));
			}
			catch{}
			return numPartido;
		}

		private TextBox buscarCuadroPartido(int numPartido)
		{
			TextBox partido;
			int numControl=0;
			string nombreControl="";
			for(int i = 0; i < tpSimple.Controls.Count; i++)
			{
				//usamos el "as" para convertir un obejto al tipo que queramos
				//Si el objeto es de ese tipo se convierte y se asigna a la
				//variable, si no, su valor sera null (Nothing in VB)
				partido= tpSimple.Controls[i] as TextBox;
				//si el objeto no esta vacio, tenemos el tipo de objeto que 
				//buscamos...
				if(partido!= null)
				{
					nombreControl=partido.Name;
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

		private void resultadoCambiado(object sender, System.EventArgs e)
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
			int numPartido=buscarNumPartido(partido);
			string col="";
			TextBox t;
			for(int i=0;i<VariablesGlobales.NumeroPartidos;i++)
			{
				t=buscarCuadroPartido(i+1);
				if(t.Text.Trim().Length>0)
					col+=t.Text;
				else
					col+="*";
			}
			txtColGanadora.Text=col;
		}

		private void columnaCambiada(object sender, System.EventArgs e)
		{
			TextBox t;
			int actual=0;
			string[] resultados={" ","1","X","2","*"};
			string col=txtColGanadora.Text;
			string signo;
			bool hayError=false;
			for(int i=0;i<col.Length;i++)
			{
				signo=col.Substring(i,1);
				if(signo.Trim().Length==0) signo="*";
				actual=Array.IndexOf(resultados,signo);
				if(actual<0)
				{
					hayError=true;
					col.Replace(signo,"*");
				}
				else
				{
					t=buscarCuadroPartido(i+1);
					t.Text=signo;
				}
			}
			if(hayError==true) MessageBox.Show("Había carácteres no válidos en la columna que han sido eliminados.","Free1X2",MessageBoxButtons.OK,MessageBoxIcon.Information);
		}

		private void crearDataset()
		{
			DataRow myDataRow;
			string[] resultado;
			dsJornadas=new DataSet("Resultados");
			// Crea la estructura del dataset
			DataTable myDataTable = new DataTable("Resultados");
			//Temporada
			DataColumn myDataColumn = new DataColumn();
			myDataColumn.DataType = System.Type.GetType("System.Int16");
			myDataColumn.ColumnName = "Temp";
			myDataTable.Columns.Add(myDataColumn);
			// Jornada
			myDataColumn = new DataColumn();
			myDataColumn.DataType = System.Type.GetType("System.Int16");
			myDataColumn.ColumnName = "Jorn";
			myDataTable.Columns.Add(myDataColumn);
			//Quiniela
			myDataColumn = new DataColumn();
			myDataColumn.DataType = System.Type.GetType("System.String");
			myDataColumn.ColumnName = "Quiniela";
			myDataColumn.MaxLength=15;
			myDataTable.Columns.Add(myDataColumn);
			// Abre el fichero de texto con los resultados
			StreamReader sr = new StreamReader( Application.StartupPath + "/Jornadas/Resultados.txt" );
			string linea="";
			while( sr.Peek() >= 0 )
			{
				// Lee la linea y la almacena en una matriz
				linea = sr.ReadLine();
				resultado=linea.Split((char)9);
				// Añade filas
				myDataRow = myDataTable.NewRow();
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

		private void tabControl1_SelectedIndexChanged(object sender, System.EventArgs e)
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

		private void btnVerArch_Click(object sender, System.EventArgs e)
		{
			fd.Filter="";
			fd.FileNames.Initialize();
			fd.ShowDialog();
			if(fd.FileName.Length>0)
			{
				System.IO.FileInfo fileInfo= new System.IO.FileInfo(fd.FileName);
				txtNombreArchBase.Text=fileInfo.Name.ToString();
			}
		}

		private void btnBuscarCarpeta_Click(object sender, System.EventArgs e)
		{
			if(fd.FileName.Length==0)
				fbd.SelectedPath=Application.StartupPath+"/Columnas/";
			else
				fbd.SelectedPath=Path.GetDirectoryName(fd.FileName);
			DialogResult dr=fbd.ShowDialog();
			if(dr==DialogResult.OK)
				lblCarpeta.Text=fbd.SelectedPath;
		}

		private void incluirPrefijo_click(object sender, System.EventArgs e)
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

		private void btnPosiblesPremios_Click(object sender, System.EventArgs e)
		{
			PosiblesPremiosFrm f=new PosiblesPremiosFrm();
			f.Show();
		}

	}
}
