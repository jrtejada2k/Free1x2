using System;
using System.IO;
using System.Windows.Forms;
using System.Collections;

using Free1X2.MotorCalculo;
using Free1X2.EntradaSalida;

namespace Free1X2.UI
{
	public class CalculaColumnasMultipleFrm : System.Windows.Forms.Form
    {
        private System.Windows.Forms.Button btnCalcular;
        private System.Windows.Forms.Button btnCancelar;
		
		private Analizador analizador;
        private System.Windows.Forms.Button btnSelArch;
		private System.Windows.Forms.ProgressBar progressBar;
		private System.Windows.Forms.ProgressBar progressBarArchivos;
		private long colsMaximas;
		
		private Timer myTimer;
		private System.Windows.Forms.ListBox listaFicheros;
		private ArrayList combinaciones=new ArrayList();
        private ArrayList ficherosSalida = new ArrayList();
        private Label label13;
        private Label label14;
        private Label label12;
        private Label label11;
        private Label label10;
        private Label label9;
        private Label label8;
        private Label label1;
        private Label label7;
        private Label colMaximoCoste;
        private Label colEstimadasCoste;
        private Label lblColsMaximo;
        private Label lblColsEstimadas;
        private Label lblSeg;
        private Label label2;
        private Label label;
        private Label horaFinal;
        private Label horaComienzo;
        private Label label6;
        private Label label5;
        private Label label4;
        private Label label3;
        private Label colAceptadaCoste;
        private Label colProcesadaCoste;
        private Label lblColsAdmitidas;
        private Label lblPorcentaje;
        private Label lblColsProcesadas;
		private bool procesoEnMarcha=false;
		
		public CalculaColumnasMultipleFrm()
		{			
			InitializeComponent();
            FormulariosHelper fH = new FormulariosHelper();
            fH.Traducir(this);
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
			procesoEnMarcha=false;
			myTimer.Stop();		
		}
		
		protected void TimerEventProcessor(Object myObject, EventArgs myEventArgs)
		{			
			ActualizaDatosCalculo();						
		}
		
		protected void calcularCols()
		{
			int dobles=0;
			int triples=0;
			string p="";
			for(int i=0;i<analizador.Pronosticos.Length;i++)
			{
				p=analizador.Pronosticos[i].Replace(",","");
				if(p.Length==2) dobles++;
				if(p.Length==3) triples++;
			}
			colsMaximas=Convert.ToInt32(Math.Pow(2,dobles)*Math.Pow(3,triples));
		}

		protected void actualizaColumnasPrevistas()
		{
			if(analizador.ArchivoColumnasBase.Length>0)
			{
				// Hay un filtro
                IArchivoColumnas f = new ArchivoColumnasTexto(analizador.ArchivoColumnasBase);
				string carpeta=Path.GetDirectoryName(analizador.ArchivoColumnasBase);
				System.IO.DirectoryInfo d=new DirectoryInfo(carpeta);
				System.IO.FileInfo[] fic=d.GetFiles(Path.GetFileName(analizador.ArchivoColumnasBase));
				if(fic.Length>0)
					colsMaximas=f.ObtenNumCols();
				else
				{
					MessageBox.Show("No se ha encontrado el filtro "+analizador.ArchivoColumnasBase+". Se calculan las columnas sin filtro.","Free1X2",MessageBoxButtons.OK,MessageBoxIcon.Exclamation);
					analizador.ArchivoColumnasBase="";
					calcularCols();
				}
				f.Cerrar();
			}
			else
			{
				calcularCols();
			}
			lblColsMaximo.Text =colsMaximas.ToString("#,##0;0");
			double costeMaximo=colsMaximas*0.5;
			colMaximoCoste.Text =costeMaximo.ToString("€ #,##0.00;0.0");
		}

		protected void ActualizaDatosCalculo()
		{
			if(procesoEnMarcha==false) return;
			// Columnas procesadas
			lblColsProcesadas.Text = analizador.ColsAnalizadas.ToString("#,##0;0");
            double cost = (analizador.ColsAnalizadas * VariablesGlobales.PrecioApuesta);			
			colProcesadaCoste.Text = cost.ToString("€ #,##0.00;0.0");
			progressBar.Value = Convert.ToInt16((analizador.ColsAnalizadas*100)/colsMaximas);		
			// Columnas aceptadas
			lblColsAdmitidas.Text = analizador.ColsAceptadas.ToString("#,##0;0");
            cost = (analizador.ColsAceptadas * VariablesGlobales.PrecioApuesta);
			colAceptadaCoste.Text = cost.ToString("€ #,##0.00;0.0");
			double porcentaje = (analizador.ColsAceptadas * 100.0/analizador.ColsAnalizadas);
			lblPorcentaje.Text = porcentaje.ToString("#,##0.00;0.00") + " %";
			// Valores estimados
			double colsEstimadas=Math.Round((colsMaximas*porcentaje)/100,0);
			lblColsEstimadas.Text = colsEstimadas.ToString("#,##0;0");
            cost = (colsEstimadas * VariablesGlobales.PrecioApuesta);
			colEstimadasCoste.Text = cost.ToString("€ #,##0.00;0.0");
		}

		void BtnCalcularClick(object sender, System.EventArgs e)
		{
			procesoEnMarcha=false;
			horaFinal.Text = "";
			lblSeg.Text = "";
			string carpetaDestino=Application.StartupPath+"/Columnas/";
			string ficheroOrigen, ficheroDestino;
			btnCalcular.Enabled = false;
			progressBar.Value=0;
			progressBarArchivos.Value=0;
			progressBar.Visible=true;
			progressBarArchivos.Visible=true;
			
			// Lee los ficheros de entrada y transforma el nombre al de salida
			ficherosSalida.Clear();
			string ext=".txt";
			for(int i=0;i<combinaciones.Count;i++)
			{
				ficheroOrigen=(string)combinaciones[i];
				ficheroDestino=carpetaDestino+Path.GetFileName(ficheroOrigen);
				// Cambia las extensiones
				ficheroDestino=ficheroDestino.Replace(".comb",ext);
				ficheroDestino=ficheroDestino.Replace(".xml",ext);
				ficherosSalida.Add(ficheroDestino);
			}
			// Con la lista de ficheros de destino, comprueba antes si ya existen y si es así, pide confirmación
			System.IO.DirectoryInfo d=new DirectoryInfo(carpetaDestino);
			System.IO.FileInfo[] f;
			for(int i=0;i<combinaciones.Count;i++)
			{
				ficheroDestino=Path.GetFileName((string)ficherosSalida[i]);
				f=d.GetFiles(ficheroDestino);
				if(f.Length>0)
				{
					// Se ha encontrado fichero. ¿Reemplazar?
					if(MessageBox.Show("El fichero "+ficheroDestino+" ya existe. ¿Deseas reemplazarlo?","Free1X2",MessageBoxButtons.YesNo,MessageBoxIcon.Question)==DialogResult.No)
					{
						// No reemplazar. Se quita de los ficheros de origen, destino y lista.
						combinaciones.RemoveAt(i);
						ficherosSalida.RemoveAt(i);
						listaFicheros.Items.RemoveAt(i);
						i--;
					}
				}
			}

			DateTime dt1 = DateTime.Now;
			DateTime dtTemp1= DateTime.Now;
			DateTime dtTemp2;
			horaComienzo.Text = dt1.ToLongTimeString();
			InicializaTimer();
			ArchivoCombinacion archComb;
			ListViewItem li;
			ResultadosCalculoMultipleFrm form=new ResultadosCalculoMultipleFrm();;
			for(int i=0;i<ficherosSalida.Count;i++)
			{
				procesoEnMarcha=true;
				listaFicheros.SelectedIndex=i;
				progressBarArchivos.Value=((i+1)*100)/ficherosSalida.Count;
				ficheroOrigen=(string)combinaciones[i];
				ficheroDestino=(string)ficherosSalida[i];
				//leer combinacion desde archivo y la pasa al analizador
				archComb = new ArchivoCombinacion();
				archComb.AbrirArchivoCombinacion( ficheroOrigen );
				analizador = new Analizador();
				archComb.CargaControladorGrupos( analizador.CtrlGrupos );
				// Lee el filtro y los pronósticos
				archComb.LeeFiltroColumnas();
				analizador.ArchivoColumnasBase=archComb.LeeFiltroColumnas();
				analizador.Pronosticos=archComb.LeePronosticos();
				archComb.Pronosticos=analizador.Pronosticos;
				actualizaColumnasPrevistas();
				analizador.AnalizaCombinacion( ficheroDestino);
				procesoEnMarcha=false;
				dtTemp2 = DateTime.Now;
				ListViewItem.ListViewSubItem[] l=new ListViewItem.ListViewSubItem[4];
				li=new ListViewItem(Path.GetFileName(ficheroOrigen));
				l[0]=new System.Windows.Forms.ListViewItem.ListViewSubItem(li,Path.GetFileName(ficheroDestino));
				l[1]=new System.Windows.Forms.ListViewItem.ListViewSubItem(li,analizador.ColsAnalizadas.ToString());
				l[2]=new System.Windows.Forms.ListViewItem.ListViewSubItem(li,analizador.ColsAceptadas.ToString());
				l[3]=new System.Windows.Forms.ListViewItem.ListViewSubItem(li,(dtTemp2.Subtract(dtTemp1)).ToString());
				for(int j=0;j<l.Length;j++)
				{
					li.SubItems.Add(l[j]);
				}
				form.listaResumen.Items.Add(li);
				dtTemp1 = DateTime.Now;
			}
			ParaTimer();
			DateTime dt2  = DateTime.Now;
			horaFinal.Text = dt2.ToLongTimeString();
			lblSeg.Text = (dt2.Subtract(dt1)).ToString();
			// Temporalmente activamos el proceso para que actualice
			procesoEnMarcha=true;
			ActualizaDatosCalculo();
			procesoEnMarcha=false;
			btnCalcular.Enabled = true;
			progressBar.Visible=false;
			progressBarArchivos.Visible=false;
			form.listaResumen.EnsureVisible(0);
			form.ShowDialog();
		}
		
		#region designer
		void InitializeComponent() 
		{
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CalculaColumnasMultipleFrm));
            this.btnCancelar = new System.Windows.Forms.Button();
            this.btnCalcular = new System.Windows.Forms.Button();
            this.btnSelArch = new System.Windows.Forms.Button();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.progressBarArchivos = new System.Windows.Forms.ProgressBar();
            this.listaFicheros = new System.Windows.Forms.ListBox();
            this.label13 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.colMaximoCoste = new System.Windows.Forms.Label();
            this.colEstimadasCoste = new System.Windows.Forms.Label();
            this.lblColsMaximo = new System.Windows.Forms.Label();
            this.lblColsEstimadas = new System.Windows.Forms.Label();
            this.lblSeg = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label = new System.Windows.Forms.Label();
            this.horaFinal = new System.Windows.Forms.Label();
            this.horaComienzo = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.colAceptadaCoste = new System.Windows.Forms.Label();
            this.colProcesadaCoste = new System.Windows.Forms.Label();
            this.lblColsAdmitidas = new System.Windows.Forms.Label();
            this.lblPorcentaje = new System.Windows.Forms.Label();
            this.lblColsProcesadas = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnCancelar
            // 
            this.btnCancelar.BackColor = System.Drawing.Color.DarkSalmon;
            this.btnCancelar.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnCancelar.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancelar.Image = ((System.Drawing.Image)(resources.GetObject("btnCancelar.Image")));
            this.btnCancelar.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCancelar.Location = new System.Drawing.Point(179, 385);
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.Size = new System.Drawing.Size(128, 32);
            this.btnCancelar.TabIndex = 22;
            this.btnCancelar.Text = "Cancelar";
            this.btnCancelar.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnCancelar.UseVisualStyleBackColor = false;
            this.btnCancelar.Click += new System.EventHandler(this.BtnCancelarClick);
            // 
            // btnCalcular
            // 
            this.btnCalcular.BackColor = System.Drawing.Color.DarkSalmon;
            this.btnCalcular.Enabled = false;
            this.btnCalcular.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnCalcular.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCalcular.Image = ((System.Drawing.Image)(resources.GetObject("btnCalcular.Image")));
            this.btnCalcular.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCalcular.Location = new System.Drawing.Point(27, 385);
            this.btnCalcular.Name = "btnCalcular";
            this.btnCalcular.Size = new System.Drawing.Size(128, 32);
            this.btnCalcular.TabIndex = 8;
            this.btnCalcular.Text = "Calcular";
            this.btnCalcular.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnCalcular.UseVisualStyleBackColor = false;
            this.btnCalcular.Click += new System.EventHandler(this.BtnCalcularClick);
            // 
            // btnSelArch
            // 
            this.btnSelArch.BackColor = System.Drawing.Color.LightSalmon;
            this.btnSelArch.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnSelArch.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSelArch.Image = ((System.Drawing.Image)(resources.GetObject("btnSelArch.Image")));
            this.btnSelArch.Location = new System.Drawing.Point(23, 8);
            this.btnSelArch.Name = "btnSelArch";
            this.btnSelArch.Size = new System.Drawing.Size(24, 20);
            this.btnSelArch.TabIndex = 24;
            this.btnSelArch.UseVisualStyleBackColor = false;
            this.btnSelArch.Click += new System.EventHandler(this.btnSelArch_Click);
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(19, 361);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(296, 16);
            this.progressBar.Step = 1;
            this.progressBar.TabIndex = 33;
            this.progressBar.Visible = false;
            // 
            // progressBarArchivos
            // 
            this.progressBarArchivos.Location = new System.Drawing.Point(19, 337);
            this.progressBarArchivos.Name = "progressBarArchivos";
            this.progressBarArchivos.Size = new System.Drawing.Size(296, 16);
            this.progressBarArchivos.Step = 1;
            this.progressBarArchivos.TabIndex = 34;
            this.progressBarArchivos.Visible = false;
            // 
            // listaFicheros
            // 
            this.listaFicheros.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.listaFicheros.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listaFicheros.Location = new System.Drawing.Point(48, 8);
            this.listaFicheros.Name = "listaFicheros";
            this.listaFicheros.ScrollAlwaysVisible = true;
            this.listaFicheros.Size = new System.Drawing.Size(272, 106);
            this.listaFicheros.TabIndex = 35;
            // 
            // label13
            // 
            this.label13.BackColor = System.Drawing.Color.White;
            this.label13.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label13.Location = new System.Drawing.Point(216, 261);
            this.label13.Name = "label13";
            this.label13.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.label13.Size = new System.Drawing.Size(107, 16);
            this.label13.TabIndex = 70;
            // 
            // label14
            // 
            this.label14.BackColor = System.Drawing.Color.White;
            this.label14.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label14.Location = new System.Drawing.Point(127, 261);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(88, 16);
            this.label14.TabIndex = 69;
            // 
            // label12
            // 
            this.label12.BackColor = System.Drawing.Color.White;
            this.label12.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label12.Location = new System.Drawing.Point(216, 244);
            this.label12.Name = "label12";
            this.label12.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.label12.Size = new System.Drawing.Size(107, 16);
            this.label12.TabIndex = 68;
            // 
            // label11
            // 
            this.label11.BackColor = System.Drawing.Color.White;
            this.label11.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.Location = new System.Drawing.Point(127, 244);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(88, 16);
            this.label11.TabIndex = 67;
            // 
            // label10
            // 
            this.label10.BackColor = System.Drawing.Color.NavajoWhite;
            this.label10.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(31, 261);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(95, 16);
            this.label10.TabIndex = 66;
            this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label9
            // 
            this.label9.BackColor = System.Drawing.Color.NavajoWhite;
            this.label9.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(31, 244);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(95, 16);
            this.label9.TabIndex = 65;
            this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label8
            // 
            this.label8.BackColor = System.Drawing.Color.NavajoWhite;
            this.label8.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(31, 193);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(95, 16);
            this.label8.TabIndex = 64;
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.Color.NavajoWhite;
            this.label1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(31, 227);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(95, 16);
            this.label1.TabIndex = 63;
            this.label1.Text = "Máximo";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label7
            // 
            this.label7.BackColor = System.Drawing.Color.NavajoWhite;
            this.label7.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(31, 210);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(95, 16);
            this.label7.TabIndex = 62;
            this.label7.Text = "Estimadas";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // colMaximoCoste
            // 
            this.colMaximoCoste.BackColor = System.Drawing.Color.White;
            this.colMaximoCoste.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.colMaximoCoste.Location = new System.Drawing.Point(216, 227);
            this.colMaximoCoste.Name = "colMaximoCoste";
            this.colMaximoCoste.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.colMaximoCoste.Size = new System.Drawing.Size(107, 16);
            this.colMaximoCoste.TabIndex = 61;
            this.colMaximoCoste.Text = "0.0";
            this.colMaximoCoste.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // colEstimadasCoste
            // 
            this.colEstimadasCoste.BackColor = System.Drawing.Color.White;
            this.colEstimadasCoste.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.colEstimadasCoste.Location = new System.Drawing.Point(216, 210);
            this.colEstimadasCoste.Name = "colEstimadasCoste";
            this.colEstimadasCoste.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.colEstimadasCoste.Size = new System.Drawing.Size(107, 16);
            this.colEstimadasCoste.TabIndex = 60;
            this.colEstimadasCoste.Text = "0.0";
            this.colEstimadasCoste.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblColsMaximo
            // 
            this.lblColsMaximo.BackColor = System.Drawing.Color.White;
            this.lblColsMaximo.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblColsMaximo.Location = new System.Drawing.Point(127, 227);
            this.lblColsMaximo.Name = "lblColsMaximo";
            this.lblColsMaximo.Size = new System.Drawing.Size(88, 16);
            this.lblColsMaximo.TabIndex = 59;
            this.lblColsMaximo.Text = "0";
            this.lblColsMaximo.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblColsEstimadas
            // 
            this.lblColsEstimadas.BackColor = System.Drawing.Color.White;
            this.lblColsEstimadas.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblColsEstimadas.Location = new System.Drawing.Point(127, 210);
            this.lblColsEstimadas.Name = "lblColsEstimadas";
            this.lblColsEstimadas.Size = new System.Drawing.Size(88, 16);
            this.lblColsEstimadas.TabIndex = 58;
            this.lblColsEstimadas.Text = "0";
            this.lblColsEstimadas.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblSeg
            // 
            this.lblSeg.BackColor = System.Drawing.Color.White;
            this.lblSeg.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSeg.Location = new System.Drawing.Point(216, 278);
            this.lblSeg.Name = "lblSeg";
            this.lblSeg.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblSeg.Size = new System.Drawing.Size(107, 16);
            this.lblSeg.TabIndex = 57;
            this.lblSeg.Text = "0.0";
            this.lblSeg.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label2
            // 
            this.label2.BackColor = System.Drawing.Color.NavajoWhite;
            this.label2.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(31, 295);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(95, 16);
            this.label2.TabIndex = 56;
            this.label2.Text = "Final";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label
            // 
            this.label.BackColor = System.Drawing.Color.NavajoWhite;
            this.label.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label.Location = new System.Drawing.Point(31, 278);
            this.label.Name = "label";
            this.label.Size = new System.Drawing.Size(95, 16);
            this.label.TabIndex = 55;
            this.label.Text = "Comienzo";
            this.label.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // horaFinal
            // 
            this.horaFinal.BackColor = System.Drawing.Color.White;
            this.horaFinal.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.horaFinal.Location = new System.Drawing.Point(127, 295);
            this.horaFinal.Name = "horaFinal";
            this.horaFinal.Size = new System.Drawing.Size(196, 16);
            this.horaFinal.TabIndex = 54;
            this.horaFinal.Text = "00:00:00";
            // 
            // horaComienzo
            // 
            this.horaComienzo.BackColor = System.Drawing.Color.White;
            this.horaComienzo.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.horaComienzo.Location = new System.Drawing.Point(127, 278);
            this.horaComienzo.Name = "horaComienzo";
            this.horaComienzo.Size = new System.Drawing.Size(88, 16);
            this.horaComienzo.TabIndex = 53;
            this.horaComienzo.Text = "00:00:00";
            // 
            // label6
            // 
            this.label6.BackColor = System.Drawing.Color.NavajoWhite;
            this.label6.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(216, 142);
            this.label6.Name = "label6";
            this.label6.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.label6.Size = new System.Drawing.Size(107, 16);
            this.label6.TabIndex = 52;
            this.label6.Text = "Coste";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label5
            // 
            this.label5.BackColor = System.Drawing.Color.NavajoWhite;
            this.label5.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(127, 142);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(88, 16);
            this.label5.TabIndex = 51;
            this.label5.Text = "Columnas";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label4
            // 
            this.label4.BackColor = System.Drawing.Color.NavajoWhite;
            this.label4.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(31, 176);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(95, 16);
            this.label4.TabIndex = 50;
            this.label4.Text = "Aceptadas";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label3
            // 
            this.label3.BackColor = System.Drawing.Color.NavajoWhite;
            this.label3.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(31, 159);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(95, 16);
            this.label3.TabIndex = 49;
            this.label3.Text = "Procesadas";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // colAceptadaCoste
            // 
            this.colAceptadaCoste.BackColor = System.Drawing.Color.White;
            this.colAceptadaCoste.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.colAceptadaCoste.Location = new System.Drawing.Point(216, 176);
            this.colAceptadaCoste.Name = "colAceptadaCoste";
            this.colAceptadaCoste.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.colAceptadaCoste.Size = new System.Drawing.Size(107, 16);
            this.colAceptadaCoste.TabIndex = 48;
            this.colAceptadaCoste.Text = "0.0";
            this.colAceptadaCoste.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // colProcesadaCoste
            // 
            this.colProcesadaCoste.BackColor = System.Drawing.Color.White;
            this.colProcesadaCoste.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.colProcesadaCoste.Location = new System.Drawing.Point(216, 159);
            this.colProcesadaCoste.Name = "colProcesadaCoste";
            this.colProcesadaCoste.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.colProcesadaCoste.Size = new System.Drawing.Size(107, 16);
            this.colProcesadaCoste.TabIndex = 47;
            this.colProcesadaCoste.Text = "0.0";
            this.colProcesadaCoste.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblColsAdmitidas
            // 
            this.lblColsAdmitidas.BackColor = System.Drawing.Color.White;
            this.lblColsAdmitidas.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblColsAdmitidas.Location = new System.Drawing.Point(127, 176);
            this.lblColsAdmitidas.Name = "lblColsAdmitidas";
            this.lblColsAdmitidas.Size = new System.Drawing.Size(88, 16);
            this.lblColsAdmitidas.TabIndex = 46;
            this.lblColsAdmitidas.Text = "0";
            this.lblColsAdmitidas.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblPorcentaje
            // 
            this.lblPorcentaje.BackColor = System.Drawing.Color.White;
            this.lblPorcentaje.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPorcentaje.Location = new System.Drawing.Point(127, 193);
            this.lblPorcentaje.Name = "lblPorcentaje";
            this.lblPorcentaje.Size = new System.Drawing.Size(88, 16);
            this.lblPorcentaje.TabIndex = 45;
            this.lblPorcentaje.Text = "0.00 %";
            this.lblPorcentaje.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblColsProcesadas
            // 
            this.lblColsProcesadas.BackColor = System.Drawing.Color.White;
            this.lblColsProcesadas.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblColsProcesadas.Location = new System.Drawing.Point(127, 159);
            this.lblColsProcesadas.Name = "lblColsProcesadas";
            this.lblColsProcesadas.Size = new System.Drawing.Size(88, 16);
            this.lblColsProcesadas.TabIndex = 44;
            this.lblColsProcesadas.Text = "0";
            this.lblColsProcesadas.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // CalculaColumnasMultipleFrm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.Color.Bisque;
            this.ClientSize = new System.Drawing.Size(342, 432);
            this.ControlBox = false;
            this.Controls.Add(this.label13);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.colMaximoCoste);
            this.Controls.Add(this.colEstimadasCoste);
            this.Controls.Add(this.lblColsMaximo);
            this.Controls.Add(this.lblColsEstimadas);
            this.Controls.Add(this.lblSeg);
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
            this.Controls.Add(this.lblPorcentaje);
            this.Controls.Add(this.lblColsProcesadas);
            this.Controls.Add(this.listaFicheros);
            this.Controls.Add(this.progressBarArchivos);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.btnSelArch);
            this.Controls.Add(this.btnCancelar);
            this.Controls.Add(this.btnCalcular);
            this.Name = "CalculaColumnasMultipleFrm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "Calcular Múltiples Combinaciones";
            this.ResumeLayout(false);

		}
		
		#endregion

		void BtnCancelarClick(object sender, System.EventArgs e)
		{
            if (analizador == null)
            {
                analizador = new Analizador();
            }
			analizador.PararAnalisis();
			progressBar.Visible=false;
			progressBarArchivos.Visible=false;
			this.Close();	
		}

		private void btnSelArch_Click(object sender, System.EventArgs e)
		{
			// Prepara el cuadro de diálogo.
			OpenFileDialog abreFiltroDialog = new OpenFileDialog();
			abreFiltroDialog.InitialDirectory = Application.StartupPath + "/Combinaciones/" ;
			abreFiltroDialog.Filter = "Combinaciones(*.comb)|*.comb|Combinaciones(*.xml)|*.xml|Todos los archivos (*.*)|*.*" ;
			abreFiltroDialog.Multiselect=true;
			if(abreFiltroDialog.ShowDialog() == DialogResult.OK)
			{
				// Elimina la lista de ficheros anteriores
				combinaciones.Clear();
				listaFicheros.Items.Clear();
				for(int i=0;i<abreFiltroDialog.FileNames.Length ;i++)
				{
					combinaciones.Add(abreFiltroDialog.FileNames[i]);
				}
				// Ordena las combinaciones y las añade al listbox
				combinaciones.Sort();
				for(int i=0;i<combinaciones.Count ;i++)
				{
					listaFicheros.Items.Add(Path.GetFileName((string)combinaciones[i]));
				}
				btnCalcular.Enabled = true;
			}
			else
				btnCalcular.Enabled = false;
		}

	}
}
