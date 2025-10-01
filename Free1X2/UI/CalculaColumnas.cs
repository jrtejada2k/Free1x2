using System;
using System.IO;
using System.Windows.Forms;
using Free1X2.EntradaSalida;
using Free1X2.MotorCalculo;

namespace Free1X2.UI
{
	public class CalculaColumnasFrm : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Label label;
		private System.Windows.Forms.Label horaFinal;
		private System.Windows.Forms.Label lblColsProcesadas;
		private System.Windows.Forms.Label horaComienzo;
		private System.Windows.Forms.Label lblSeg;
		private System.Windows.Forms.Button btnCalcular;
		private System.Windows.Forms.Label colProcesadaCoste;
		private System.Windows.Forms.Button btnCancelar;
		private System.Windows.Forms.Label lblPorcentaje;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label lblColsAdmitidas;
		private System.Windows.Forms.Label colAceptadaCoste;
		
		private Analizador analizador;
		private string archivoResultados = "";
		private string[] equipos;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label colMaximoCoste;
		private System.Windows.Forms.Label colEstimadasCoste;
		private System.Windows.Forms.Label lblColsMaximo;
		private System.Windows.Forms.Label lblColsEstimadas;
		private long colsMaximas;
		private System.Windows.Forms.ProgressBar progressBar;
		private int numFiltroCols=0;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.RadioButton chckCalcular;
		private System.Windows.Forms.RadioButton chckAnalizar;
		private System.Windows.Forms.RadioButton chckGrabar;
		private System.Windows.Forms.Label lblNombreArch;
		private System.Windows.Forms.Button btnSelArch;
		private System.Windows.Forms.CheckBox chckPleno;
        private StatusStrip statusStrip1;
        private ToolStripStatusLabel lblEstado;
        private Label label8;
        private Label label9;
        private Label label10;
        private Label label11;
        private Label label12;
        private Label label13;
        private Label label14;
		
		private Timer myTimer;
		
		public CalculaColumnasFrm(Analizador analizador, string[] arrayEquipos)
		{			
			InitializeComponent();
			this.analizador = analizador;
			equipos=arrayEquipos;
            FormulariosHelper fH = new FormulariosHelper();
            fH.Traducir(this);
		}

		public string ObtenNombreArchivoResultados()
		{
			string nombreArchivo = "";
			
			SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.InitialDirectory = Application.StartupPath + "/Columnas/";
			saveDialog.Filter = "Columnas(*.txt)|*.txt|Todos los archivos (*.*)|*.*" ;

			if(saveDialog.ShowDialog() == DialogResult.OK)
			{	
				nombreArchivo = saveDialog.FileName;
				numFiltroCols=saveDialog.FilterIndex;
			}
		
			return nombreArchivo;
		}
		
		protected void InicializaTimer()
		{		
			myTimer = new Timer();
			
			// Adds the event and the event handler for the method that will 
			// process the timer event to the timer.
			myTimer.Tick += new EventHandler(TimerEventProcessor);

			
			// Sets the timer interval to 0.5 seconds.
			myTimer.Interval = 500;
			myTimer.Start();		
		}
		
		protected void ParaTimer()
		{
			myTimer.Stop();		
		}
		
		protected void TimerEventProcessor(Object myObject, EventArgs myEventArgs)
		{
			ActualizaDatosCalculo();						
		}

        private bool HayConflictosEntreArchivos()
        {
            bool hayConflictos = false;
            for (int i = 0; i < analizador.CtrlGrupos.GruposPartidos.Count; i++)
            {
                Grupo grup = analizador.CtrlGrupos.GruposPartidos[i];
                if (!grup.EsGrupoBase)
                {
                    if (grup.UsaFiltroParcial)
                    {
                        if (archivoResultados == grup.ArchivoFiltroGrupo)
                        {
                            hayConflictos = true;
                            break;
                        }
                    }
                }
                else
                {
                    if (archivoResultados == analizador.ArchivoColumnasBase)
                    {
                        hayConflictos = true;
                        break;
                    }
                }                
            }
            return hayConflictos;
        }

		protected void ActualizaDatosCalculo()
		{
			// Columnas procesadas
			lblColsProcesadas.Text = analizador.ColsAnalizadas.ToString("#,##0;0");
			double cost = (analizador.ColsAnalizadas * VariablesGlobales.PrecioApuesta);			
			colProcesadaCoste.Text = cost.ToString(VariablesGlobales.Moneda + "#,##0.00;0.0");
            try
            {
                progressBar.Value = Convert.ToInt16((analizador.ColsAnalizadas * 100) / colsMaximas);
            }
            catch
            {
                progressBar.Value = 0;
            }
            // Columnas aceptadas
			lblColsAdmitidas.Text = analizador.ColsAceptadas.ToString("#,##0;0");
			cost = (analizador.ColsAceptadas * VariablesGlobales.PrecioApuesta);
            colAceptadaCoste.Text = cost.ToString(VariablesGlobales.Moneda + "#,##0.00;0.0");
			double porcentaje = (analizador.ColsAceptadas * 100.0/analizador.ColsAnalizadas);
			lblPorcentaje.Text = porcentaje.ToString("#,##0.00;0.00") + " %";
			// Valores estimados
			double colsEstimadas=Math.Round((colsMaximas*porcentaje)/100,0);
			lblColsEstimadas.Text = colsEstimadas.ToString("#,##0;0");
            cost = (colsEstimadas * VariablesGlobales.PrecioApuesta);
            colEstimadasCoste.Text = cost.ToString(VariablesGlobales.Moneda + "#,##0.00;0.0");
		}

		void BtnCalcularClick(object sender, System.EventArgs e)
        {
            for (int i = 0; i < analizador.CtrlGrupos.GruposPartidos.Count; i++)
            {
                Grupo grup = analizador.CtrlGrupos.GruposPartidos[i];
                if (!grup.EsGrupoBase)
                {
                    if (grup.UsaFiltroParcial)
                    {
                        lblEstado.Text = "Cargando Filtros Parciales";
                        Application.DoEvents();
                    }
                }
            }
            lblEstado.Text = "Calculando...";
			btnCalcular.Enabled = false;
			progressBar.Visible=true;
			
			DateTime dt1  = DateTime.Now;
			horaComienzo.Text = dt1.ToLongTimeString();
			guardarCombinacionTemporal();
			InicializaTimer();

			if(this.chckGrabar.Checked)
			{ 
                if (!HayConflictosEntreArchivos())
                {
                    analizador.AnalizaCombinacion(archivoResultados);
                }
                else
                {
                    MessageBox.Show("No puede usar como archivo de resultados un archivo usado ya en la combinación", "Error", MessageBoxButtons.OK);
                    Close();
                }
			}
			else if(this.chckAnalizar.Checked)
			{
                if (chckPleno.Checked)
                {
                    analizador.AnalizaCombinacion(15);
                }
                else
                {
                    analizador.AnalizaCombinacion(14);
                }
			}
			else
				analizador.AnalizaCombinacion(false);

			ParaTimer();
			
			DateTime dt2  = DateTime.Now;
			horaFinal.Text = dt2.ToLongTimeString();
			lblSeg.Text = (dt2.Subtract(dt1)).ToString();
			ActualizaDatosCalculo();
			
			btnCalcular.Enabled = true;
			progressBar.Visible=false;

            lblEstado.Text = "Listo";
		}

        private void guardarCombinacionTemporal()
	{
		if(analizador.HayDatos==false) return;
		string nombreCorto=DateTime.Now.ToShortDateString()+"_"+DateTime.Now.ToShortTimeString()+"_tmp.comb";
		nombreCorto=nombreCorto.Replace("/","-");
		nombreCorto=nombreCorto.Replace(":","-");
        nombreCorto = nombreCorto.Replace("\\", "-");
		string nombreArchivoComb=Application.StartupPath;
		nombreArchivoComb+="/Temp/"+nombreCorto;
		ArchivoCombinacion archComb = new ArchivoCombinacion();
		archComb.NombreArchivo = nombreArchivoComb;
		archComb.Equipos = equipos;
		archComb.Pronosticos = analizador.Pronosticos;
		archComb.ArchivoColumnasFiltro = analizador.ArchivoColumnasBase;
		archComb.Grupos = analizador.GruposPartidos;
		archComb.CtrlGrupos = analizador.CtrlGrupos;
		archComb.IfThen=analizador.IfThen;
		archComb.GuardaArchivo();
	}
		
		#region designer
		void InitializeComponent() 
		{
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CalculaColumnasFrm));
            this.colAceptadaCoste = new System.Windows.Forms.Label();
            this.lblColsAdmitidas = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.lblPorcentaje = new System.Windows.Forms.Label();
            this.btnCancelar = new System.Windows.Forms.Button();
            this.colProcesadaCoste = new System.Windows.Forms.Label();
            this.btnCalcular = new System.Windows.Forms.Button();
            this.lblSeg = new System.Windows.Forms.Label();
            this.horaComienzo = new System.Windows.Forms.Label();
            this.lblColsProcesadas = new System.Windows.Forms.Label();
            this.horaFinal = new System.Windows.Forms.Label();
            this.label = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.colMaximoCoste = new System.Windows.Forms.Label();
            this.colEstimadasCoste = new System.Windows.Forms.Label();
            this.lblColsMaximo = new System.Windows.Forms.Label();
            this.lblColsEstimadas = new System.Windows.Forms.Label();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.chckPleno = new System.Windows.Forms.CheckBox();
            this.lblNombreArch = new System.Windows.Forms.Label();
            this.btnSelArch = new System.Windows.Forms.Button();
            this.chckGrabar = new System.Windows.Forms.RadioButton();
            this.chckAnalizar = new System.Windows.Forms.RadioButton();
            this.chckCalcular = new System.Windows.Forms.RadioButton();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.lblEstado = new System.Windows.Forms.ToolStripStatusLabel();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // colAceptadaCoste
            // 
            this.colAceptadaCoste.BackColor = System.Drawing.Color.White;
            this.colAceptadaCoste.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.colAceptadaCoste.Location = new System.Drawing.Point(216, 162);
            this.colAceptadaCoste.Name = "colAceptadaCoste";
            this.colAceptadaCoste.Size = new System.Drawing.Size(107, 16);
            this.colAceptadaCoste.TabIndex = 11;
            this.colAceptadaCoste.Text = "0.0";
            this.colAceptadaCoste.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblColsAdmitidas
            // 
            this.lblColsAdmitidas.BackColor = System.Drawing.Color.White;
            this.lblColsAdmitidas.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblColsAdmitidas.Location = new System.Drawing.Point(127, 162);
            this.lblColsAdmitidas.Name = "lblColsAdmitidas";
            this.lblColsAdmitidas.Size = new System.Drawing.Size(88, 16);
            this.lblColsAdmitidas.TabIndex = 9;
            this.lblColsAdmitidas.Text = "0";
            this.lblColsAdmitidas.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label4
            // 
            this.label4.BackColor = System.Drawing.Color.NavajoWhite;
            this.label4.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(31, 162);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(95, 16);
            this.label4.TabIndex = 13;
            this.label4.Text = "Aceptadas";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label5
            // 
            this.label5.BackColor = System.Drawing.Color.NavajoWhite;
            this.label5.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(127, 128);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(88, 16);
            this.label5.TabIndex = 14;
            this.label5.Text = "Columnas";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label6
            // 
            this.label6.BackColor = System.Drawing.Color.NavajoWhite;
            this.label6.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(216, 128);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(107, 16);
            this.label6.TabIndex = 15;
            this.label6.Text = "Coste";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label2
            // 
            this.label2.BackColor = System.Drawing.Color.NavajoWhite;
            this.label2.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(31, 281);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(95, 16);
            this.label2.TabIndex = 21;
            this.label2.Text = "Final";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label3
            // 
            this.label3.BackColor = System.Drawing.Color.NavajoWhite;
            this.label3.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(31, 145);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(95, 16);
            this.label3.TabIndex = 12;
            this.label3.Text = "Procesadas";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblPorcentaje
            // 
            this.lblPorcentaje.BackColor = System.Drawing.Color.White;
            this.lblPorcentaje.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPorcentaje.Location = new System.Drawing.Point(127, 179);
            this.lblPorcentaje.Name = "lblPorcentaje";
            this.lblPorcentaje.Size = new System.Drawing.Size(88, 16);
            this.lblPorcentaje.TabIndex = 7;
            this.lblPorcentaje.Text = "0.00 %";
            this.lblPorcentaje.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // btnCancelar
            // 
            this.btnCancelar.BackColor = System.Drawing.Color.DarkSalmon;
            this.btnCancelar.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnCancelar.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancelar.Image = ((System.Drawing.Image)(resources.GetObject("btnCancelar.Image")));
            this.btnCancelar.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCancelar.Location = new System.Drawing.Point(179, 352);
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.Size = new System.Drawing.Size(128, 32);
            this.btnCancelar.TabIndex = 22;
            this.btnCancelar.Text = "Cancelar";
            this.btnCancelar.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnCancelar.UseVisualStyleBackColor = false;
            this.btnCancelar.Click += new System.EventHandler(this.BtnCancelarClick);
            // 
            // colProcesadaCoste
            // 
            this.colProcesadaCoste.BackColor = System.Drawing.Color.White;
            this.colProcesadaCoste.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.colProcesadaCoste.Location = new System.Drawing.Point(216, 145);
            this.colProcesadaCoste.Name = "colProcesadaCoste";
            this.colProcesadaCoste.Size = new System.Drawing.Size(107, 16);
            this.colProcesadaCoste.TabIndex = 10;
            this.colProcesadaCoste.Text = "0.0";
            this.colProcesadaCoste.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // btnCalcular
            // 
            this.btnCalcular.BackColor = System.Drawing.Color.DarkSalmon;
            this.btnCalcular.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnCalcular.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCalcular.Image = ((System.Drawing.Image)(resources.GetObject("btnCalcular.Image")));
            this.btnCalcular.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCalcular.Location = new System.Drawing.Point(27, 352);
            this.btnCalcular.Name = "btnCalcular";
            this.btnCalcular.Size = new System.Drawing.Size(128, 32);
            this.btnCalcular.TabIndex = 8;
            this.btnCalcular.Text = "Calcular";
            this.btnCalcular.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnCalcular.UseVisualStyleBackColor = false;
            this.btnCalcular.Click += new System.EventHandler(this.BtnCalcularClick);
            // 
            // lblSeg
            // 
            this.lblSeg.BackColor = System.Drawing.Color.White;
            this.lblSeg.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSeg.Location = new System.Drawing.Point(216, 264);
            this.lblSeg.Name = "lblSeg";
            this.lblSeg.Size = new System.Drawing.Size(107, 16);
            this.lblSeg.TabIndex = 23;
            this.lblSeg.Text = "0.0";
            this.lblSeg.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // horaComienzo
            // 
            this.horaComienzo.BackColor = System.Drawing.Color.White;
            this.horaComienzo.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.horaComienzo.Location = new System.Drawing.Point(127, 264);
            this.horaComienzo.Name = "horaComienzo";
            this.horaComienzo.Size = new System.Drawing.Size(88, 16);
            this.horaComienzo.TabIndex = 17;
            this.horaComienzo.Text = "00:00:00";
            // 
            // lblColsProcesadas
            // 
            this.lblColsProcesadas.BackColor = System.Drawing.Color.White;
            this.lblColsProcesadas.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblColsProcesadas.Location = new System.Drawing.Point(127, 145);
            this.lblColsProcesadas.Name = "lblColsProcesadas";
            this.lblColsProcesadas.Size = new System.Drawing.Size(88, 16);
            this.lblColsProcesadas.TabIndex = 5;
            this.lblColsProcesadas.Text = "0";
            this.lblColsProcesadas.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // horaFinal
            // 
            this.horaFinal.BackColor = System.Drawing.Color.White;
            this.horaFinal.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.horaFinal.Location = new System.Drawing.Point(127, 281);
            this.horaFinal.Name = "horaFinal";
            this.horaFinal.Size = new System.Drawing.Size(196, 16);
            this.horaFinal.TabIndex = 19;
            this.horaFinal.Text = "00:00:00";
            // 
            // label
            // 
            this.label.BackColor = System.Drawing.Color.NavajoWhite;
            this.label.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label.Location = new System.Drawing.Point(31, 264);
            this.label.Name = "label";
            this.label.Size = new System.Drawing.Size(95, 16);
            this.label.TabIndex = 20;
            this.label.Text = "Comienzo";
            this.label.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.Color.NavajoWhite;
            this.label1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(31, 213);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(95, 16);
            this.label1.TabIndex = 32;
            this.label1.Text = "Máximo";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label7
            // 
            this.label7.BackColor = System.Drawing.Color.NavajoWhite;
            this.label7.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(31, 196);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(95, 16);
            this.label7.TabIndex = 31;
            this.label7.Text = "Estimadas";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // colMaximoCoste
            // 
            this.colMaximoCoste.BackColor = System.Drawing.Color.White;
            this.colMaximoCoste.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.colMaximoCoste.Location = new System.Drawing.Point(216, 213);
            this.colMaximoCoste.Name = "colMaximoCoste";
            this.colMaximoCoste.Size = new System.Drawing.Size(107, 16);
            this.colMaximoCoste.TabIndex = 30;
            this.colMaximoCoste.Text = "0.0";
            this.colMaximoCoste.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // colEstimadasCoste
            // 
            this.colEstimadasCoste.BackColor = System.Drawing.Color.White;
            this.colEstimadasCoste.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.colEstimadasCoste.Location = new System.Drawing.Point(216, 196);
            this.colEstimadasCoste.Name = "colEstimadasCoste";
            this.colEstimadasCoste.Size = new System.Drawing.Size(107, 16);
            this.colEstimadasCoste.TabIndex = 29;
            this.colEstimadasCoste.Text = "0.0";
            this.colEstimadasCoste.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblColsMaximo
            // 
            this.lblColsMaximo.BackColor = System.Drawing.Color.White;
            this.lblColsMaximo.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblColsMaximo.Location = new System.Drawing.Point(127, 213);
            this.lblColsMaximo.Name = "lblColsMaximo";
            this.lblColsMaximo.Size = new System.Drawing.Size(88, 16);
            this.lblColsMaximo.TabIndex = 28;
            this.lblColsMaximo.Text = "0";
            this.lblColsMaximo.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblColsEstimadas
            // 
            this.lblColsEstimadas.BackColor = System.Drawing.Color.White;
            this.lblColsEstimadas.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblColsEstimadas.Location = new System.Drawing.Point(127, 196);
            this.lblColsEstimadas.Name = "lblColsEstimadas";
            this.lblColsEstimadas.Size = new System.Drawing.Size(88, 16);
            this.lblColsEstimadas.TabIndex = 27;
            this.lblColsEstimadas.Text = "0";
            this.lblColsEstimadas.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(19, 328);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(296, 16);
            this.progressBar.Step = 1;
            this.progressBar.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar.TabIndex = 33;
            this.progressBar.Visible = false;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.chckPleno);
            this.groupBox1.Controls.Add(this.lblNombreArch);
            this.groupBox1.Controls.Add(this.btnSelArch);
            this.groupBox1.Controls.Add(this.chckGrabar);
            this.groupBox1.Controls.Add(this.chckAnalizar);
            this.groupBox1.Controls.Add(this.chckCalcular);
            this.groupBox1.Location = new System.Drawing.Point(19, 8);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(304, 104);
            this.groupBox1.TabIndex = 35;
            this.groupBox1.TabStop = false;
            // 
            // chckPleno
            // 
            this.chckPleno.Enabled = false;
            this.chckPleno.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chckPleno.Location = new System.Drawing.Point(154, 32);
            this.chckPleno.Name = "chckPleno";
            this.chckPleno.Size = new System.Drawing.Size(134, 16);
            this.chckPleno.TabIndex = 29;
            this.chckPleno.Text = "Incluir pleno al 15";
            // 
            // lblNombreArch
            // 
            this.lblNombreArch.Enabled = false;
            this.lblNombreArch.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNombreArch.Location = new System.Drawing.Point(32, 80);
            this.lblNombreArch.Name = "lblNombreArch";
            this.lblNombreArch.Size = new System.Drawing.Size(264, 16);
            this.lblNombreArch.TabIndex = 28;
            this.lblNombreArch.Text = "(Seleccionar)";
            this.lblNombreArch.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnSelArch
            // 
            this.btnSelArch.BackColor = System.Drawing.Color.Silver;
            this.btnSelArch.Enabled = false;
            this.btnSelArch.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnSelArch.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSelArch.Image = ((System.Drawing.Image)(resources.GetObject("btnSelArch.Image")));
            this.btnSelArch.Location = new System.Drawing.Point(8, 77);
            this.btnSelArch.Name = "btnSelArch";
            this.btnSelArch.Size = new System.Drawing.Size(24, 20);
            this.btnSelArch.TabIndex = 27;
            this.btnSelArch.UseVisualStyleBackColor = false;
            this.btnSelArch.Click += new System.EventHandler(this.btnSelArch_Click);
            this.btnSelArch.EnabledChanged += new System.EventHandler(this.btnSelArch_EnabledChanged);
            // 
            // chckGrabar
            // 
            this.chckGrabar.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chckGrabar.Location = new System.Drawing.Point(8, 54);
            this.chckGrabar.Name = "chckGrabar";
            this.chckGrabar.Size = new System.Drawing.Size(140, 16);
            this.chckGrabar.TabIndex = 2;
            this.chckGrabar.Text = "Grabar archivo";
            this.chckGrabar.CheckedChanged += new System.EventHandler(this.chckGrabar_CheckedChanged);
            // 
            // chckAnalizar
            // 
            this.chckAnalizar.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chckAnalizar.Location = new System.Drawing.Point(8, 31);
            this.chckAnalizar.Name = "chckAnalizar";
            this.chckAnalizar.Size = new System.Drawing.Size(140, 16);
            this.chckAnalizar.TabIndex = 1;
            this.chckAnalizar.Text = "Calcular y analizar";
            this.chckAnalizar.CheckedChanged += new System.EventHandler(this.chckAnalizar_CheckedChanged);
            // 
            // chckCalcular
            // 
            this.chckCalcular.Checked = true;
            this.chckCalcular.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chckCalcular.Location = new System.Drawing.Point(8, 8);
            this.chckCalcular.Name = "chckCalcular";
            this.chckCalcular.Size = new System.Drawing.Size(140, 16);
            this.chckCalcular.TabIndex = 0;
            this.chckCalcular.TabStop = true;
            this.chckCalcular.Text = "Calcular";
            // 
            // statusStrip1
            // 
            this.statusStrip1.BackColor = System.Drawing.Color.Bisque;
            this.statusStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Visible;
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblEstado});
            this.statusStrip1.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.Flow;
            this.statusStrip1.Location = new System.Drawing.Point(0, 401);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            this.statusStrip1.Size = new System.Drawing.Size(342, 18);
            this.statusStrip1.SizingGrip = false;
            this.statusStrip1.TabIndex = 36;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // lblEstado
            // 
            this.lblEstado.Name = "lblEstado";
            this.lblEstado.Size = new System.Drawing.Size(57, 13);
            this.lblEstado.Text = "Preparado";
            // 
            // label8
            // 
            this.label8.BackColor = System.Drawing.Color.NavajoWhite;
            this.label8.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(31, 179);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(95, 16);
            this.label8.TabIndex = 37;
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label9
            // 
            this.label9.BackColor = System.Drawing.Color.NavajoWhite;
            this.label9.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(31, 230);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(95, 16);
            this.label9.TabIndex = 38;
            this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label10
            // 
            this.label10.BackColor = System.Drawing.Color.NavajoWhite;
            this.label10.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(31, 247);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(95, 16);
            this.label10.TabIndex = 39;
            this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label11
            // 
            this.label11.BackColor = System.Drawing.Color.White;
            this.label11.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.Location = new System.Drawing.Point(127, 230);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(88, 16);
            this.label11.TabIndex = 40;
            // 
            // label12
            // 
            this.label12.BackColor = System.Drawing.Color.White;
            this.label12.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label12.Location = new System.Drawing.Point(216, 230);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(107, 16);
            this.label12.TabIndex = 41;
            // 
            // label13
            // 
            this.label13.BackColor = System.Drawing.Color.White;
            this.label13.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label13.Location = new System.Drawing.Point(216, 247);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(107, 16);
            this.label13.TabIndex = 43;
            // 
            // label14
            // 
            this.label14.BackColor = System.Drawing.Color.White;
            this.label14.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label14.Location = new System.Drawing.Point(127, 247);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(88, 16);
            this.label14.TabIndex = 42;
            // 
            // CalculaColumnasFrm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.Color.Bisque;
            this.ClientSize = new System.Drawing.Size(342, 419);
            this.ControlBox = false;
            this.Controls.Add(this.label13);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.colMaximoCoste);
            this.Controls.Add(this.colEstimadasCoste);
            this.Controls.Add(this.lblColsMaximo);
            this.Controls.Add(this.lblColsEstimadas);
            this.Controls.Add(this.lblSeg);
            this.Controls.Add(this.btnCancelar);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label);
            this.Controls.Add(this.horaFinal);
            this.Controls.Add(this.horaComienzo);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.colAceptadaCoste);
            this.Controls.Add(this.colProcesadaCoste);
            this.Controls.Add(this.lblColsAdmitidas);
            this.Controls.Add(this.btnCalcular);
            this.Controls.Add(this.lblPorcentaje);
            this.Controls.Add(this.lblColsProcesadas);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CalculaColumnasFrm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Calcular columnas";
            this.Load += new System.EventHandler(this.CalculaColumnasFrm_Load);
            this.groupBox1.ResumeLayout(false);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		
		#endregion

		void BtnCancelarClick(object sender, System.EventArgs e)
		{
			analizador.PararAnalisis();
			progressBar.Visible=false;
			Close();	
		}

		private void chckGrabar_CheckedChanged(object sender, System.EventArgs e)
		{
			if(this.chckGrabar.Checked)
			{
				this.btnSelArch.Enabled = true;
				this.lblNombreArch.Enabled = true;

				if(archivoResultados == "")
				{
					this.btnCalcular.Enabled = false;
				}
				
			}
			else
			{
				this.btnSelArch.Enabled = false;
				this.lblNombreArch.Enabled = false;
				
				this.btnCalcular.Enabled = true;
			}
		}

		private void btnSelArch_Click(object sender, System.EventArgs e)
		{
			string tempNombre = ObtenNombreArchivoResultados();
			
			if(tempNombre != "")
			{
				archivoResultados = tempNombre;
				lblNombreArch.Text = Path.GetFileName(archivoResultados);
				
				btnCalcular.Enabled = true;
			}				
		}

		private void btnSelArch_EnabledChanged(object sender, System.EventArgs e)
		{
			FormulariosHelper f=new FormulariosHelper();
			f.CambiarFondoBoton(btnSelArch);
		}

		private void CalculaColumnasFrm_Load(object sender, System.EventArgs e)
		{
			if(analizador.ArchivoColumnasBase.Length>0)
			{
				// Hay un filtro
                IArchivoColumnas f = new ArchivoColumnasTexto(analizador.ArchivoColumnasBase);
				colsMaximas=f.ObtenNumCols();
				f.Cerrar();
			}
			else
			{
				int dobles=0;
				int triples=0;
			    for(int i=0;i<analizador.Pronosticos.Length;i++)
				{
					string p = analizador.Pronosticos[i].Replace(",","");
					if(p.Length==2) dobles++;
					if(p.Length==3) triples++;
				}
				colsMaximas=Convert.ToInt32(Math.Pow(2,dobles)*Math.Pow(3,triples));
			}
			lblColsMaximo.Text =colsMaximas.ToString("#,##0;0");
			double costeMaximo=colsMaximas*VariablesGlobales.PrecioApuesta;
            colMaximoCoste.Text = costeMaximo.ToString(VariablesGlobales.Moneda + "#,##0.00;0.0");
		}

		private void chckAnalizar_CheckedChanged(object sender, System.EventArgs e)
		{
			chckPleno.Enabled=chckAnalizar.Checked;
		}		
	}
}
