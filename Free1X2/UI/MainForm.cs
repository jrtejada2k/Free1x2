using System;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using Free1X2.EntradaSalida;
using Free1X2.MotorCalculo;
using Free1X2.UI.Controls;
using Free1X2.UI.Estadisticas;
using Free1X2.UI.Filtros;
using Free1X2.UI.Modern.Theming;

namespace Free1X2.UI 
{
	public partial class MainForm : Form
	{
        #region Propiedades
        
        private string nombreArchivoComb = "";
        private string archivoFiltroCols = "";
        private System.ComponentModel.IContainer components;
        private int grupoPantalla;			
        private string boletoOnline = "";
        public Analizador analizador = new Analizador();			
	    readonly string version = "Free1X2 - Versión " + Application.ProductVersion + " Rarotonga";	
		
        public string BoletoOnline
        {
            get { return boletoOnline; }
            set { boletoOnline = value; }
        }

		public Analizador MotorCalculo
		{
			get{ return analizador; }
		}

		public int NoGrupoPantalla
		{
			get{ return grupoPantalla; }
        }		
		
        #endregion

        #region Constructor and Initialization

        public MainForm()
        {
            InitializeComponent();
            InicializarBarrasHerramientas();
            PonerNombrePrograma(nombreArchivoComb);
            // Notifications disabled for performance
            var fH = new FormulariosHelper();
            fH.Traducir(this);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            ModernTheme.ApplyToForm(this);
        }
		
        private void InicializarBarrasHerramientas()
        {
            //Obtiene las preferencias de inicio
            tsFree.Visible = free1X2ToolStripMenuItem.Checked = VariablesGlobales.MostrarTsFree;
            tsOperaciones.Visible = operacionesToolStripMenuItem.Checked = VariablesGlobales.MostrarTsOperaciones;
            tsCombinacion.Visible = combinacionToolStripMenuItem.Checked = VariablesGlobales.MostrarTsCombinacion;
            tsArchivo.Visible = archivoToolStripMenuItem.Checked = VariablesGlobales.MostrarTsArchivo;
            tsFiltros.Visible = filtrosToolStripMenuItem.Checked = VariablesGlobales.MostrarTsFiltros;
            tsUtilidades.Visible = utilidadesToolStripMenuItem.Checked = VariablesGlobales.MostrarTsUtilidades;

            ObtenerPosicionBarraHerramientas();
        }
		
        private void ObtenerPosicionBarraHerramientas()
        {
            ToolStrip[] arrayBarras = { tsFree, tsArchivo, tsOperaciones, tsCombinacion, tsFiltros, tsUtilidades};
            //Vaciar el ToolStripContainer
            toolStripContainer4.TopToolStripPanel.Controls.Remove(tsFiltros);
            toolStripContainer4.TopToolStripPanel.Controls.Remove(tsFree);
            toolStripContainer4.TopToolStripPanel.Controls.Remove(tsOperaciones);
            toolStripContainer4.TopToolStripPanel.Controls.Remove(tsCombinacion);
            toolStripContainer4.TopToolStripPanel.Controls.Remove(tsUtilidades);
            toolStripContainer4.TopToolStripPanel.Controls.Remove(tsArchivo);

            int x = 3;
            int y = 24;

            for (int i = 0; i < arrayBarras.Length; i++)
            {
                ToolStrip tstrp = arrayBarras[i];
                if (tstrp.Visible)
                {
                    tstrp.Location = new Point(x, y);
                    toolStripContainer4.TopToolStripPanel.Controls.Add(tstrp);

                    int anchoTotal = x + tstrp.Width + 1;
                    x += tstrp.Width +1;
                    
                    if (anchoTotal >= toolStripContainer4.Width)
                    {
                        x = 3;
                        y += 200;
                        toolStripContainer4.TopToolStripPanel.Height += y;
                    }                   
                }
            }
        }
		
		protected void PonerNombrePrograma(string nombreArchivoEnUso)
		{
			if(nombreArchivoEnUso == "")
			{
				Text = version;
			}
			else
			{
				Text = version + " - " + Path.GetFileNameWithoutExtension( nombreArchivoEnUso );		
			}
		}
		
		#endregion

		#region Eventos Load, Activated y Closed
		
		void MainFormLoad(object sender, EventArgs e)
		{
			ConfiguraBoleto();
            // Auto-updater disabled for performance

            // Advertising banner disabled for performance
            pBQuinielista.Visible = false;			
		}
		void ConfiguraBoleto()
		{
            //Aqui mete un grupo sin tener que hacerlo
			pronosticos.NumPartidos=VariablesGlobales.NumeroPartidos;
			pronosticos.InicializarBoleto(VariablesGlobales.NumeroPartidos, VariablesGlobales.Separador);
		}	

		// Auto-updater system removed for performance
		
		void MainFormActivated(object sender, EventArgs e)
		{
			ActualizaGrupoPantalla();
		}

		private void MainForm_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
            if (!VariablesGlobales.FuncionaBajoMono)
            {
                if (!Salir())
                    e.Cancel = true;
            }
		}	
        private bool Salir()
        {
            bool salir = true;
            if (VariablesGlobales.PedirConfirmacionAlSalir)
            {
                var salirFrm = new SalirFrm();
                salirFrm.ShowDialog();
                salir = salirFrm.exit;
                salirFrm.Dispose();
            }
            if (salir)
            {
                // Borra los posibles archivos temporales
                var d = new DirectoryInfo(Application.StartupPath + "/Temp/");
                FileInfo[] f = d.GetFiles();
                for (int i = 0; i < f.Length; i++)
                {
                    if (f[i].Name != "temp" && f[i].Name.IndexOf("_tmp.comb") < 0) f[i].Delete();
                }
                //Guarda las preferencias de barras de herramientas
                var ac = new AConfiguracion();
                ac.GuardarToolBarsVisibles(tsFree.Visible, tsFiltros.Visible, tsCombinacion.Visible, tsOperaciones.Visible, tsArchivo.Visible, tsUtilidades.Visible);
                Application.Exit();
            }
            return salir;
        }		
		
		#endregion		

        #region Menu Free1X2
		
		private void MSalir(object sender, EventArgs e)
		{
            if (!VariablesGlobales.FuncionaBajoMono)
            {
                Salir();
            }
            else
            {
                Application.Exit();
            }
		}
		
		private void MConfiguracion(object sender, EventArgs e)
		{
			var f=new ConfiguracionFrm();
			f.ShowDialog();
		}
		
        private void bConfAnalisis_Click(object sender, EventArgs e)
        {
            var configAnalisis = new ConfiguracionAnalisisFrm();
            configAnalisis.ShowDialog();
        }
        private void configurarAnálisisToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var configAnalisis = new ConfiguracionAnalisisFrm();
            configAnalisis.ShowDialog();
        }	
		
		private void MAyuda(object sender, EventArgs e)
		{
            var ayudaFrm = new AyudaFrm();
            ayudaFrm.ShowDialog();
		}		
		
		private void mAcercaDe(object sender, EventArgs e)
		{
			var f=new AcercaDeFrm();
			f.ShowDialog();
		}
		
        private void comprobarActualizacionesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Auto-update feature disabled.\nPlease check manually for updates.", "Update Check", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
		
		#endregion		
		
		#region Menu Archivo
		
		void MAbreBoleto(object sender, EventArgs e)
		{
			AbrirPartidosBoleto();
		}	
		protected void AbrirPartidosBoleto()
		{
			pronosticos.LeerBoletoBase();
		}
		
		void MGuardarPartidosClick(object sender, EventArgs e)
		{
			GuardarPartidosBoleto();
		}	
		protected void GuardarPartidosBoleto()
		{
			pronosticos.CrearArchivoBoleto();
		}		
		
        private void obtenerBoletosOnlineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var descargaBoleto = new DescargaBoletoFrm(this);
            descargaBoleto.ShowDialog();

            if (boletoOnline != "")
            {
                pronosticos.LeerBoletoBase(boletoOnline);
            }
        }		
		
		private void MNuevaComb(object sender, EventArgs e)
		{
			// Comprueba si hay una combinación activa
			if(nombreArchivoComb.Equals(""))
			{
				// En ese caso, pide confirmación. Si la respuesta es no, sale.
				if(MessageBox.Show("Hay una combinación activa. ¿Seguro de crear una combinación nueva?","Nueva combinación",MessageBoxButtons.YesNo,MessageBoxIcon.Question)!=DialogResult.Yes)
				{
					return;
				}
			}
			var archComb= new ArchivoCombinacion();
			pronosticos.SetEquiposVacio();
            pronosticos.Reiniciar14Triples();
			// Desactivamos el filtro si lo hay
			DesactivarFiltroColumnas();
			// Carga los grupos (sólo boleto base)
			analizador = new Analizador();
			archComb.CargaControladorGruposVacio( analizador.CtrlGrupos );
			grupoPantalla = 0;
			CambiaGrupoSeleccionado(0);
			ActualizaGrupoPantalla();
			PonerNombrePrograma( "" );
			pronosticos.NombreGrupo="";
            nombreArchivoComb = "";
		}
		
	    void MAbrirCombClick(object sender, EventArgs e)
		{
			// Comprueba si hay una combinación activa
			if(!nombreArchivoComb.Equals(""))
			{
				// En ese caso, pide confirmación. Si la respuesta es no, sale.
				if(MessageBox.Show("Ya hay una combinación abierta. ¿Seguro de sustituirla?","Abrir combinación",MessageBoxButtons.YesNo,MessageBoxIcon.Question)!=DialogResult.Yes)
				{
					return;
				}
			}
			AbreCombinacion();			
		}	
		protected void AbreCombinacion()
		{
			var abreCombDialog = new OpenFileDialog();
			abreCombDialog.InitialDirectory = "Combinaciones\\" ;
            abreCombDialog.Filter = "Todas las combinaciones(*.comb, *.xml)|*.comb; *.xml|Combinaciones(*.comb)|*.comb|Combinaciones(*.xml)|*.xml|Todos los archivos (*.*)|*.*";
					
			if(abreCombDialog.ShowDialog() == DialogResult.OK)
			{
				nombreArchivoComb = abreCombDialog.FileName;
		    	
				//leer combinacion desde archivo
				var archComb = new ArchivoCombinacion();
				archComb.AbrirArchivoCombinacion( nombreArchivoComb );

				string[] equipos = archComb.LeeEquipos();
				pronosticos.SetEquipos(equipos);
				string archFiltroCols = archComb.LeeFiltroColumnas();

				if(archFiltroCols != "")
				{
					ActivaFiltroColumnas( archFiltroCols );
				}	
				else
				{
					DesactivarFiltroColumnas();
				}
				
				analizador = new Analizador();
				archComb.CargaControladorGrupos( analizador.CtrlGrupos );
				analizador.IfThen= archComb.CargaIfThen();
								
				//anadir grupos activos!!
				PonerPronosticosPartidosGrupoActivos();
		    	
				PonerPronosticosPantalla( archComb.LeePronosticos() );
		    	
				grupoPantalla = 0;
				CambiaGrupoSeleccionado(0);
				ActualizaGrupoPantalla();	
		    	
				PonerNombrePrograma( nombreArchivoComb );
			}
		}		
		protected void PonerPronosticosPantalla( string[] arrayPronosticos )
		{
            for (int i = 0; i < arrayPronosticos.Length; i++)
            {
                pronosticos[i + 1] = arrayPronosticos[i];
            }
		}		
				
		void MGuardarComb(object sender, EventArgs e)
		{
			GuardaCombinacion();
		}	
		protected void GuardaCombinacion()
		{
			//actualiza pronosticos y grupos partidos en el analizador
			ObtenDatosGruposPronosticos();
			
			if( nombreArchivoComb.Equals("") )
			{
				//obten nombre nueva combinacion
				var saveDialog = new SaveFileDialog
				                     {
				                         InitialDirectory = "Combinaciones\\",
				                         Filter =
				                             "Combinaciones(*.comb)|*.comb|Combinaciones(*.xml)|*.xml|Todas las combinaciones(*.comb, *.xml)|*.comb; *.xml|Todos los archivos (*.*)|*.*"
				                     };

			    if(saveDialog.ShowDialog() == DialogResult.OK)
				{	
					nombreArchivoComb = saveDialog.FileName;
				}			
			}
			
			if( !nombreArchivoComb.Equals("") )
			{
				var archComb = new ArchivoCombinacion
				                   {
				                       NombreArchivo = nombreArchivoComb,
				                       Equipos = pronosticos.DevolverEquipos(),
				                       Pronosticos = analizador.Pronosticos,
				                       ArchivoColumnasFiltro = archivoFiltroCols,
				                       Grupos = analizador.GruposPartidos,
				                       CtrlGrupos = analizador.CtrlGrupos,
				                       IfThen = analizador.IfThen
				                   };
			    archComb.GuardaArchivo();

				PonerNombrePrograma( nombreArchivoComb );
			}		
		}		
		
		void MGuardarCombComo(object sender, EventArgs e)
		{
			//obten nombre nueva combinacion
			var saveDialog = new SaveFileDialog
			                     {
			                         InitialDirectory = "Combinaciones\\",
			                         Filter =
			                             "Combinaciones(*.comb)|*.comb|Combinaciones(*.xml)|*.xml|Todas las combinaciones(*.comb, *.xml)|*.comb; *.xml|Todos los archivos (*.*)|*.*"
			                     };
		    if(saveDialog.ShowDialog() == DialogResult.OK)
			{	
				nombreArchivoComb = saveDialog.FileName;
				GuardaCombinacion();
			}			
		}		

        private void MBorrarCombsTemp(object sender, EventArgs e)
		{
			// Borra las combinaciones temporales
			if(MessageBox.Show("¿Borrar las combinaciones temporales?","Free1X2",MessageBoxButtons.YesNo,MessageBoxIcon.Question)==DialogResult.No)
				return;
			var d=new DirectoryInfo(Application.StartupPath+"/Temp/");
			FileInfo[] f=d.GetFiles();
			for(int i=0;i<f.Length;i++)
			{
				if(f[i].Name.IndexOf("_tmp.comb")>=0) f[i].Delete();
			}
		}	
		
        private void borrarInformesDeErrorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("¿Borrar todos los Informes generados?", "Free1X2", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                return;
            var d = new DirectoryInfo(Application.StartupPath + "/Informes/");
            FileInfo[] f = d.GetFiles();
            for (int i = 0; i < f.Length; i++)
            {
                if (f[i].Name.Length > 7)
                {
                    string prefix = f[i].Name.Substring(0, 7);
                    if (prefix == "Informe")
                    {
                        f[i].Delete();
                    }
                }
            }
        }

        private void gestiónDeEquiposToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var gestor = new GestorEquiposFrm();
            gestor.ShowDialog();
        }		
		
		#endregion
				
		#region Menu Ver
		
        private void filtrosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItemClick(tsFiltros, filtrosToolStripMenuItem);
        }	

        private void free1X2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItemClick(tsFree, free1X2ToolStripMenuItem);
        }

        private void operacionesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItemClick(tsOperaciones, operacionesToolStripMenuItem);
        }

        private void utilidadesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItemClick(tsUtilidades, utilidadesToolStripMenuItem);
        }

        private void combinaciónToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItemClick(tsCombinacion, combinacionToolStripMenuItem);
        }

        private void archivoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItemClick(tsArchivo, archivoToolStripMenuItem);
        }

        private void ToolStripMenuItemClick(ToolStrip toolStrip, ToolStripMenuItem toolStripMenuItem)
        {
            if (toolStrip.Visible)
            {
                toolStrip.Visible = false;
                toolStripMenuItem.Checked = false;
            }
            else
            {
                toolStrip.Visible = true;
                toolStripMenuItem.Checked = true;
            }

            ObtenerPosicionBarraHerramientas();
        }

        private void listadoDeCondicionesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ObtenDatosGruposPronosticos();
            var listaCond = new ListadoCondicionesFrm
                                {
                                    Equipos = pronosticos.DevolverEquipos(),
                                    Pronosticos = analizador.Pronosticos,
                                    ArchivoFiltro = archivoFiltroCols,
                                    GrupoDePartidos = analizador.GruposPartidos,
                                    ControladorDeGrupos = analizador.CtrlGrupos,
                                    ControladorDeIfThen = analizador.IfThen
                                };
            listaCond.Show();
        }
		
		#endregion
		
		#region Menu Combinacion

		void MCalcular(object sender, EventArgs e)
		{
			AbreCalculoColumnasFrm( false );
		}		
		protected void AbreCalculoColumnasFrm( bool guardaColumnas )
		{
			ObtenDatosGruposPronosticos();
            if (ComprobarPronosticos())
            {
                if (archivoFiltroCols != "" && chkFiltroCols.Estado == CtrSemaforo.NombreEstado.Verde)
                {
                    analizador.ArchivoColumnasBase = archivoFiltroCols;
                    if (txtCompletarCon.Visible)
                    {
                        analizador.CompletarCon = txtCompletarCon.Text;
                    }
                }
                else
                {
                    analizador.ArchivoColumnasBase = "";
                }

                var calcFrm = new CalculaColumnasFrm(analizador, pronosticos.DevolverEquipos());
                calcFrm.ShowDialog();
                ActualizaGrupoPantalla();
            }
		}	
		protected void ObtenDatosGruposPronosticos()
		{
			ObtenPronosticos();
			ObtenPartidosGrupos();
		}	
		protected void ObtenPronosticos()
		{
            for (int i = 0; i < pronosticos.NumPartidos; i++)
            {
                analizador.SetPronostico(i, pronosticos[i + 1]);
            }
		}	
		protected void ObtenPartidosGrupos()
		{
		    for(int i=0; i < analizador.GruposPartidos.Count; i++)
			{
				Grupo grupo = analizador.GruposPartidos[ i ];
				grupo.Partidos = pronosticos.ObtenPartidosGrupo( i );
                
				if(grupoPantalla==i) grupo.NombreGrupo=pronosticos.NombreGrupo;
			}
		}
        protected bool ComprobarPronosticos()
        {
            bool esValido = true;
            for (int i = 1; i <= pronosticos.NumPartidos; i++)
            {
                if (pronosticos[i] == "")
                {
                    MessageBox.Show("No puede dejar el partido " + i+ " sin pronóstico", "Error");
                    esValido = false;
                    break;
                }
            } return esValido;
        }		
		protected void ActualizaGrupoPantalla()
		{
			Grupo grupo = analizador.GruposPartidos[ grupoPantalla ];
			if(grupo.EsGrupoBase || grupoPantalla==0) grupo.NombreGrupo="";
			if(pronosticos.txtNombre.Tag.ToString()=="G")
			{
				pronosticos.txtNombre.TextAlign=HorizontalAlignment.Center;
			}
			else
			{
				pronosticos.NombreGrupo=grupo.NombreGrupo;
				pronosticos.txtNombre.TextAlign=HorizontalAlignment.Left;
			}

		    IFiltro filtro = grupo.GetFiltro( Filtro.NoVariantes.ToString());			
			PonerColorBotonCondicion( btnNoVariantes, checkNoVariantes, filtro);
			
			filtro = grupo.GetFiltro( Filtro.SignosSeguidos.ToString());
			PonerColorBotonCondicion( btnSignosSeguidos, checkSigSeguidos, filtro);
			
			filtro = grupo.GetFiltro( Filtro.Dibujos.ToString());
			PonerColorBotonCondicion( btnDibujos, checkDibujos, filtro);	
			
			filtro = grupo.GetFiltro( Filtro.ColProbables.ToString());
			PonerColorBotonCondicion( btnCP, checkCP, filtro);
			
			filtro = grupo.GetFiltro( Filtro.PesosNumericos.ToString());
			PonerColorBotonCondicion( btnPesosNum, checkPesosNum, filtro);

			filtro = grupo.GetFiltro( Filtro.ValoracionSignos.ToString());
			PonerColorBotonCondicion( btnValoracion, checkValoracion, filtro);

			filtro = grupo.GetFiltro( Filtro.NoInterrupciones.ToString());
			PonerColorBotonCondicion( btnIterrupciones, checkInterrupciones, filtro);

			filtro = grupo.GetFiltro( Filtro.Distancias.ToString());
			PonerColorBotonCondicion( btnDistancias, checkDistancias, filtro);

			filtro = grupo.GetFiltro( Filtro.GruposEquipos.ToString());
			PonerColorBotonCondicion( btnGruposEquipos, checkGruposEquipos, filtro);

			filtro = grupo.GetFiltro( Filtro.Contactos.ToString());
			PonerColorBotonCondicion( btnContactos, checkContactos, filtro);

			filtro = grupo.GetFiltro( Filtro.FormatosSignos.ToString());
			PonerColorBotonCondicion( btnFormatos, checkFormatos, filtro);

            filtro = grupo.GetFiltro(Filtro.Formatos123.ToString());
            PonerColorBotonCondicion(btnFormatos123, chkFormatos123, filtro);

            filtro = grupo.GetFiltro(Filtro.Simetrias.ToString());
            PonerColorBotonCondicion(btnSimetrias, checkSimetrias, filtro);

            filtro = grupo.GetFiltro(Filtro.Diferencias.ToString());
            PonerColorBotonCondicion(btnDiferencias, chkDiferencias, filtro);

			if(analizador.CtrlGrupos.ControlesGrupos.Count>1)
				SetBotonEstado(btnControlGrupos, BotonEstado.Activo);
			else
				SetBotonEstado(btnControlGrupos, BotonEstado.Inactivo);
			PonerColorBotonIfThen();

            //Indicar Filtro

            if (!grupo.EsGrupoBase || grupoPantalla != 0)
            {
                if (grupo.ArchivoFiltroGrupo != "")
                {
                    ActivaFiltroColumnasParcial(grupo.ArchivoFiltroGrupo);

                }
                else
                {
                    DesactivarFiltroColumnasParcial();
                }
                grupo.ReinicializaVariablesFiltroParcial();

            }
		}	
        protected bool ActivaFiltroColumnasParcial(string archivoFiltro)
        {
            try
            {
                bool archivoValido = false;
                IArchivoColumnas cols = new ArchivoColumnasTexto(archivoFiltro);
                int signos = cols.ObtenNumSignos();

                if (signos != VariablesGlobales.NumeroPartidos)
                {
                    //No hace nada, mostrar mensaje de error
                    MessageBox.Show("El Filtro Parcial para los grupos debe tener " + VariablesGlobales.NumeroPartidos + " partidos");
                }
                else
                {
                    chkFiltroColsParcial.Enabled = true;
                    chkFiltroColsParcial.Estado = CtrSemaforo.NombreEstado.Verde;
                    btnAbreFiltroParcial.Image = imgCerrar.Image;
                    btnAbreFiltroParcial.Tag = "-";

                    //quitar extension

                    string temp = Path.GetFileNameWithoutExtension(archivoFiltro);
                    lblNombreFiltroParcial.Text = temp;

                    archivoValido = true;
                }
                return archivoValido;
            }
            catch
            {
                return false;
            }
        }	
        protected void DesactivarFiltroColumnasParcial()
        {
            lblNombreFiltroParcial.Text = "";
            chkFiltroColsParcial.Enabled = false;
            chkFiltroColsParcial.Estado = CtrSemaforo.NombreEstado.Neutro;
            btnAbreFiltroParcial.Tag = "+";
            btnAbreFiltroParcial.Image = imgAbrir.Image;
        }
		protected void PonerColorBotonCondicion( Button boton, CtrSemaforo filtroActivo, IFiltro filtro)
		{
			if( filtro.IsActive )
			{
				SetBotonEstado(boton, BotonEstado.Activo);
				filtroActivo.Enabled = true;
				filtroActivo.Estado=CtrSemaforo.NombreEstado.Verde;
			}
			else if( filtro.ContieneDatos )
			{
				SetBotonEstado(boton, BotonEstado.Error);
				filtroActivo.Enabled = true;
				filtroActivo.Estado=CtrSemaforo.NombreEstado.Rojo;
			}
			else
			{
				SetBotonEstado(boton, BotonEstado.Inactivo);
				filtroActivo.Enabled = false;
				filtroActivo.Estado=CtrSemaforo.NombreEstado.Neutro;
			}
		}

		private enum BotonEstado { Activo, Error, Neutro, Inactivo }

		private static void SetBotonEstado(Button btn, BotonEstado estado)
		{
			switch (estado)
			{
				case BotonEstado.Activo:
					btn.BackColor = ModernTheme.Colors.Success;
					btn.ForeColor = Color.White;
					btn.FlatAppearance.BorderColor = ModernTheme.Colors.Success;
					break;
				case BotonEstado.Error:
					btn.BackColor = ModernTheme.Colors.Error;
					btn.ForeColor = Color.White;
					btn.FlatAppearance.BorderColor = ModernTheme.Colors.Error;
					break;
				case BotonEstado.Neutro:
					btn.BackColor = Color.FromArgb(255, 244, 206);
					btn.ForeColor = ModernTheme.Colors.Text;
					btn.FlatAppearance.BorderColor = Color.FromArgb(200, 160, 0);
					break;
				case BotonEstado.Inactivo:
					btn.BackColor = ModernTheme.Colors.Surface;
					btn.ForeColor = ModernTheme.Colors.Text;
					btn.FlatAppearance.BorderColor = ModernTheme.Colors.Border;
					break;
			}
		}
		
		private void MCalcularMult(object sender, EventArgs e)
		{
            if (ComprobarPronosticos())
            {
                var calcFrm = new CalculaColumnasMultipleFrm();
                calcFrm.ShowDialog();
            }
		}
		
		void MAbreVisualizadorBoletos(object sender, EventArgs e)
		{
			AbreVisualizadorBoletos();		
		}
		protected void AbreVisualizadorBoletos()
		{
			var verBoletoFrm = new VerBoletos();
			verBoletoFrm.ShowDialog();		
		}		
		private void menuItem50_Click(object sender, EventArgs e)
		{
			MAbreVisualizadorBoletos(this,EventArgs.Empty);
		}	

        private void verBoletosEnEditorDeTextoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var openFile = new OpenFileDialog
                               {
                                   Filter = "Columnas(*.txt)|*.txt|Todos los archivos (*.*)|*.*",
                                   InitialDirectory = "Columnas\\"
                               };

            if (openFile.ShowDialog() == DialogResult.OK)
            {
                IArchivoColumnas aCol = new ArchivoColumnasTexto(openFile.FileName);
                string[] cols = aCol.LeerTodasCols(false);
                var verBoletos = new VerBoletosEnEditorFrm(cols);
                verBoletos.ShowDialog();
            }
        }		

		private void MAbreImprimirBoletos(object sender, EventArgs e)
		{
			var f=new ImprimirBoletoFrm();
			f.ShowDialog();
		}	
		
		void MReducir(object sender, EventArgs e)
		{
			AbreReducciones();
		}		
		protected void AbreReducciones()
		{
			var reductor = new ReductorFrm();
			reductor.ShowDialog();
		}
	
		void MEscrutinio(object sender, EventArgs e)
		{
			AbreEscrutinios();
		}	
		protected void AbreEscrutinios()
		{
			var escrFrm = new EscrutiniosFrm();
			escrFrm.ShowDialog();			
		}
		
        private void MEscrutinioComb(object sender, EventArgs e)
        {
            AbreEscrutiniosComb();
        }		
        protected void AbreEscrutiniosComb()
        {
            var escrFrm = new EscrutiniosFrm();
            escrFrm.ShowDialog();
        }		
        private void bEscrutarComb_Click(object sender, EventArgs e)
        {
            //AbreEscrutiniosComb();
        }		

		private void mAnalizarColumnas(object sender, EventArgs e)
		{
			var f=new AnalizarFicheroFrm();
			f.ShowDialog();
		}		

		private void mAnalizaCombinacion ( object sender, EventArgs e )
		{
			//actualiza pronosticos y grupos partidos en el analizador
			ObtenDatosGruposPronosticos();
		    Point p = Cursor.Position;
			pronosticos.ObtenPronosticos();
			var f = new ColGanadoraFrm( pronosticos.NumPartidos,nombreArchivoComb, 
                                        analizador, pronosticos.ListaPronosticos)
			            {
			                StartPosition = FormStartPosition.Manual,
			                DesktopLocation = p
			            };
		    f.ShowDialog();
		}		

		void MAbreGraficoCombinacion(object sender, EventArgs e)
		{
			AbreGraficoCombinacion();		
		}		
		protected void AbreGraficoCombinacion()
		{
			var graficoColumnasFrm = new GraficoColumnasFrm();
			graficoColumnasFrm.ShowDialog();		
		}
		
		void MAnalisisSignos(object sender, EventArgs e)
		{
			var signosFrm = new VSignosFrm();
			signosFrm.ShowDialog();			
		}
		
		void MProbabilidades(object sender, EventArgs e)
		{
			AbreProbabilidadPremios();
		}	
		protected void AbreProbabilidadPremios()
		{
		    var calcFrm = new ProbabilidadPremios();
		    calcFrm.ShowDialog();
		}
		
	    private void MEstadisticas(object sender, EventArgs e)
		{
			var estadisticas = new Anastatics();
			estadisticas.ShowDialog();
		}
		
		private void MAbrePonerP15(object sender, EventArgs e)
		{
			var p15Frm = new AgregaP15Frm();
			p15Frm.ShowDialog();
		}	
		
		#endregion
		
		#region Menu Grupos
		
		private void mAbrirGrupos(object sender, EventArgs e)
		{
			if(MessageBox.Show("Si se han introducido, los datos del grupo actual se perderán. ¿Sustituir el grupo actual?","Abrir grupo",MessageBoxButtons.YesNo,MessageBoxIcon.Question,MessageBoxDefaultButton.Button2)==DialogResult.No)
				return;

			var abreCombDialog = new OpenFileDialog
			                         {
			                             InitialDirectory = "Condiciones\\",
			                             Filter = "Grupos(*.grupos)|*.grupos|Grupos(*.xml)|*.xml|Todos los archivos (*.*)|*.*"
			                         };
		    if(abreCombDialog.ShowDialog() == DialogResult.OK)
			{
				Abrir(abreCombDialog.FileName);
			}
		}

		private void Abrir(string nombreArchivo)
		{
		    int pos=grupoPantalla;
			//leer combinacion desde archivo
			var archComb = new ArchivoCondiciones();
			archComb.AbrirArchivoCombinacion( nombreArchivo );
			Grupo nuevoGrupo = MotorCalculo.CtrlGrupos.GruposPartidos[pos];
			archComb.LeeGrupos(ref nuevoGrupo);
			// Elimina el grupo y lo inserta en la misma posicion
			MotorCalculo.CtrlGrupos.GruposPartidos.BorrarGrupo(pos);
			MotorCalculo.CtrlGrupos.GruposPartidos.InsertarGrupo(nuevoGrupo,pos);
			PonerPronosticosPartidosGrupoActivos();
			CambiaGrupo( pos);
			ActualizaGrupoPantalla();
		}
		protected void PonerPronosticosPartidosGrupoActivos()
		{
			pronosticos.BorrarPartidosGrupoActivo();

		    for(int i=0; i < analizador.GruposPartidos.Count; i++)
		    {
		        Grupo grupo = analizador.GruposPartidos[ i ];
		        pronosticos.PonerPartidosGrupoActivo( grupo.Partidos );
		    }
		}
		
		private void mGuardarGrupos(object sender, EventArgs e)
		{
			// Lo primero, guarda los datos de pantalla al grupo
			ObtenDatosGruposPronosticos();
			var saveDialog = new SaveFileDialog
			                     {
			                         InitialDirectory = "Condiciones\\",
			                         Filter = "Grupos(*.grupos)|*.grupos|Grupos(*.xml)|*.xml|Todos los archivos (*.*)|*.*"
			                     };
		    if(saveDialog.ShowDialog() == DialogResult.OK)
			{	
				Guardar(saveDialog.FileName);
			}			
		}
		private void Guardar(string nombreArchivo)
		{
		    var archComb = new ArchivoCondiciones();
			archComb.NombreArchivo=nombreArchivo;
            archComb.GuardaArchivo(analizador.GruposPartidos[grupoPantalla]);
		}

		private void mCopiarGrupos(object sender, EventArgs e)
		{
			// Lo primero, guarda los datos de pantalla al grupo
			ObtenDatosGruposPronosticos();
			// Lee de un fichero temporal
			string nombreArchivo=Application.StartupPath+"/Temp/"+"tmp.grupos";
			Guardar(nombreArchivo);
			btnPegarGrupo.Enabled=true;
			mPegarGrupo.Enabled=true;
		}
		
		private void mPegarGrupos(object sender, EventArgs e)
		{
			if(MessageBox.Show("Si se han introducido, los datos del grupo actual se perderán. ¿Sustituir el grupo actual?","Pegar grupo",MessageBoxButtons.YesNo,MessageBoxIcon.Question,MessageBoxDefaultButton.Button2)==DialogResult.No)
				return;
			// Crea un fichero temporal
			Abrir(Application.StartupPath+"/Temp/"+"tmp.grupos");
		}

		private void mInsertarGrupos(object sender, EventArgs e)
		{
		    int numGrupo=grupoPantalla;
			Grupo grupo=new Grupo();
			bool[] gr = new bool[pronosticos.NumPartidos];
			analizador.GruposPartidos.InsertarGrupo(grupo, numGrupo);
			pronosticos.grupoPronosticos.Insert(numGrupo,gr);
			CambiaGrupo( numGrupo);
			pronosticos.ActivaTodosPartidos(true);
			ActualizaGrupoPantalla();
			pronosticos.ObtenTitulo();
			if(analizador.CtrlGrupos.ControlesGrupos.Count>0)
			{
				string txt = "Todos los grupos siguientes han sido movidos una posición más.";
				txt+="No olvides modificar el control de grupos si alguno resulta afectado por la inserción.";
				MessageBox.Show(txt,"ATENCION",MessageBoxButtons.OK,MessageBoxIcon.Warning);
			}
		}

		private void mEliminarGrupos(object sender, EventArgs e)
		{
		    BorraGrupoEnPantalla();
		}
        private void BorraGrupoEnPantalla()
        {
            char[] separadores = new char[] { ',', '-' };
            //Borrar grupo
            pronosticos.grupoPronosticos.RemoveAt(grupoPantalla);
            analizador.GruposPartidos.BorrarGrupo(grupoPantalla);
            for (int i = 0; i < analizador.CtrlGrupos.ControlesGrupos.Count; i++)
            {
                //El grupo que se borra es grupoPantalla, quitar el último grupo
                string[] gControlados = analizador.CtrlGrupos.ControlesGrupos[i].ObtenGruposControlados().Split(separadores);
                int[] gControladosInt = analizador.CtrlGrupos.ControlesGrupos[i].GruposControlados;
                string temp = "";
                for (int j = 0; j < gControlados.Length; j++)
                {
                    if (gControladosInt[j] > grupoPantalla)
                    {
                        temp += (gControladosInt[j] - 1);
                        temp += ",";
                    }
                    else if (gControladosInt[j] < grupoPantalla)
                    {
                        temp += gControladosInt[j];
                        temp += ",";
                    }
                }
                if (temp.Length > 1)
                {
                    string def = temp.Substring(0, temp.Length - 1);
                    string[] defString = def.Split(',');
                    gControladosInt = new int[defString.Length];
                    for (int j = 0; j < gControladosInt.Length; j++)
                    {
                        gControladosInt[j] = Convert.ToInt32(defString[j]);
                    }
                    analizador.CtrlGrupos.ControlesGrupos[i].PonerGruposControlados(def);
                    analizador.CtrlGrupos.ControlesGrupos[i].GruposControlados = gControladosInt;
                }
            }
            grupoPantalla--;
            if (analizador.CtrlGrupos.ControlesGrupos.Count > 0)
            {
                string txt = "Todos los grupos siguientes han sido movidos una posición más.";
                txt += "No olvides modificar el control de grupos si alguno resulta afectado por la inserción.";
                MessageBox.Show(txt, "ATENCION", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            CambiaGrupoSeleccionado(grupoPantalla);
        }
		
		#endregion

		#region Menu Filtros
		
		void MAbreFiltros(object sender, EventArgs e) 
		{
			var comfil = new CombinarFiltros();
			comfil.ShowDialog();
		}	

		void MDifEntreFiltros(object sender, EventArgs e)
		{
			AbreDifEntreFiltros();
		}
		protected void AbreDifEntreFiltros()
		{
			var diferencias = new DiFiltros();
			diferencias.ShowDialog();
		}
		
		void MAbreFiltroCoincidencias(object sender, EventArgs e)
		{
			AbreFiltroCoincidencias();
		}
        protected void AbreFiltroCoincidencias()
		{
			var coincidenciasFrm = new Coincidencias();
			coincidenciasFrm.ShowDialog();
		}
		
		private void mFiltroAidomnou_Click(object sender, EventArgs e)
		{
			AbreFiltroAidomnou();
		}
		private void AbreFiltroAidomnou()
		{
			var f = new aidomnou();
			f.ShowDialog();
		}
		
		private void mFiltroPim_Click(object sender, EventArgs e)
		{
			AbreFiltroPim();
		}		
		private void AbreFiltroPim()
		{
			var f = new GeneraPim();
			f.ShowDialog();
		}		
	
		#endregion
		
		#region Menu Operaciones
		
		void MCombSumar(object sender, EventArgs e)
		{
			AbreAlgebraColumnas();
		}
		protected void AbreAlgebraColumnas()
		{
			var algebraColumnasFrm = new AlgebraColumnasFrm();
			algebraColumnasFrm.ShowDialog();		
		}

		void MTransposicionFrm(object sender, EventArgs e)
		{
			var transposicionFrm = new TransposicionFrm();
			transposicionFrm.ShowDialog();			
		}
		
		void MMultiplicador(object sender, EventArgs e)
		{
			var multiplicadorFrm = new MultiplicadorFrm();
			multiplicadorFrm.ShowDialog();			
		}
		
		void MFraccionador(object sender, EventArgs e)
		{
			var fraccionadorFrm = new FraccionadorFrm();
			fraccionadorFrm.ShowDialog();
		}

		void MRotacionDeSignos(object sender, EventArgs e)
		{
			var rotacionDeSignosFrm = new RotacionDeSignosFrm();
			rotacionDeSignosFrm.ShowDialog();
		}
		
		#endregion
		
		#region Menu Utilidades
		
		void MSubeCategoria(object sender, EventArgs e)
		{
			AbreSubirCategoria();	
		}
		protected void AbreSubirCategoria()
		{
		    var subeCategoria = new SubirCategoriaFrm();
		    subeCategoria.ShowDialog();
		}
		
		void MAbreModificador(object sender, EventArgs e)
		{
			var modificador = new ModificadorFrm();
			modificador.ShowDialog();
		}
		
		void MGeneradorCol(object sender, EventArgs e)
		{
			var generarCPs = new GenerarCPs();
			generarCPs.ShowDialog();
			
		}		
		
		void MDifEntreColumnas(object sender, EventArgs e)
		{
			AbreDifEntreColumnas();
		}	
		protected void AbreDifEntreColumnas()
		{
			var diferencias = new DifCols();
			diferencias.ShowDialog();
		}		
		
		private void MProbabilidad(object sender, EventArgs e)
		{
		    var ordenarPorProbabilidadFrm = new OrdenarPorProbabilidadFrm();
		    ordenarPorProbabilidadFrm.ShowDialog() ;
		}		
		
		void MAbreSelectorJM(object sender, EventArgs e)
		{
			var selector = new SelecJM();
			selector.ShowDialog();
		}
		
		void MAbreSelectorMS(object sender, EventArgs e)
		{
			var selector = new SelectorMS();
			selector.ShowDialog();
		}		
		
	    private void MAbreRentabilidad(object sender, EventArgs e)
	    {
	        var RentFrm = new RentabilidadFrm();
	        RentFrm.ShowDialog() ;
	    }		
		
		void MAbreColumnasGEPT(object sender, EventArgs e)
		{
			AbreColumnasGEPT();
		}		
	    protected void AbreColumnasGEPT()
		{
			var GEPT = new GEPTFrm();
			GEPT.ShowDialog();		
		}

		void MAbreSeleccionTramos(object sender, EventArgs e)
		{
			AbreSeleccionTramos();
		}
		protected void AbreSeleccionTramos()
		{
			var tramificarForm = new TramificarForm();
			tramificarForm.ShowDialog();
		}

		void MAbreSelectorPremiadas(object sender, EventArgs e)
		{
			AbreSelectorPremiadas();
		}
	    protected void AbreSelectorPremiadas()
		{
			var premiadas = new PremiadasFrm();
			premiadas.ShowDialog();
		}

	    private void MEstimacion(object sender, EventArgs e)
		{
			var estimadorPremiosFrm =new EstimadorPremiosFrm();
			estimadorPremiosFrm.ShowDialog();
		}		
		
	    private void MBancoPruebas(object sender, EventArgs e)
		{
			var bancoPruebasFrm =new BancoPruebasFrm();
			bancoPruebasFrm.ShowDialog();
		}		
		
		private void MImportExport(object sender, EventArgs e)
		{
			var f=new ImportExportFrm();
			f.ShowDialog();
		}		
		
		private void menuItem54_Click(object sender, EventArgs e)
		{
			var anaCombi = new AnaCombi();
			anaCombi.ShowDialog();
		}
		
		private void MnuReduccionesPerfectas_Click(object sender, EventArgs e)
		{
			var f=new FrmReducidasPerfectas();
			f.ShowDialog();
		}		
		
		private void MDepLineal ( object sender, EventArgs e )
		{
			var f = new FrmDependenciaLineal ();
			f.ShowDialog ();
		}
		
        private void compresorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var comp = new Compresor();
            comp.ShowDialog();
        }
		
        private void estuColToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var abdForm = new EstucolFrm();
            abdForm.ShowDialog();
        }		
		
		#endregion

		#region Zona Condiciones
		
		void BtnNoVariantesClick(object sender, EventArgs e)
		{
		    var noVariantesFrm = new NoVariantesFrm( analizador.GruposPartidos[ grupoPantalla ] );
		    noVariantesFrm.ShowDialog();
		}
	    void BtnPesosNumClick(object sender, EventArgs e)
	    {
	        var pnFrm = new PesosNumFrm( analizador.GruposPartidos[ grupoPantalla ] );
	        pnFrm.ShowDialog();
	    }		
	    void BtnSignosSeguidosClick(object sender, EventArgs e)
		{
		    var signosSeguidosFrm = new SignosSeguidosFrm( analizador.GruposPartidos[ grupoPantalla ] );
		    signosSeguidosFrm.ShowDialog();
		}
		private void btnValoracion_Click(object sender, EventArgs e)
		{
		    var valFrm = new ValoracionFrm(analizador.GruposPartidos[ grupoPantalla ]);
		    valFrm.ShowDialog();
		}
		
		void BtnDibujosClick(object sender, EventArgs e)
		{
		    var dibujosFrm = new DibujosFrm( analizador.GruposPartidos[ grupoPantalla ], this);
		    dibujosFrm.ShowDialog();
		}	
	
		void BtnCPClick(object sender, EventArgs e)
		{
		    var cpFrm = new ColProbablesFrm( analizador.GruposPartidos[ grupoPantalla ], this );
		    cpFrm.ShowDialog();
		}			
		void BtnIterrupciones_Click(object sender, EventArgs e)
		{
		    var interrupciones = new InterrupcionesFrm(analizador.GruposPartidos[ grupoPantalla ], this);
		    interrupciones.ShowDialog();
		}		
		private void btnDistancias_Click(object sender, EventArgs e)
		{
			var distancias= new DistanciasFrm(analizador.GruposPartidos[grupoPantalla], this);
			distancias.ShowDialog();
		}		
		private void btnGruposEquipos_Click(object sender, EventArgs e)
		{
			var gruposEqFrm = new GruposEquiposFrm( analizador.GruposPartidos[ grupoPantalla ] , this);
			gruposEqFrm.ShowDialog();
		}		
		private void btnContactos_Click(object sender, EventArgs e)
		{
			var contactos = new ContactosFrm(analizador.GruposPartidos[ grupoPantalla ], this);
			contactos.ShowDialog();
		}		
		private void btnFormatos_Click(object sender, EventArgs e)
		{
		    var formatos = new FormatosFrm(analizador.GruposPartidos[ grupoPantalla ], this);
		    formatos.ShowDialog();
		}
	    private void btnFormatos123_Click(object sender, EventArgs e)
        {
            var formatos123Frm = new Formatos123Frm(analizador.GruposPartidos[grupoPantalla], this);
            formatos123Frm.ShowDialog();
        }		
        private void btnSimetrias_Click(object sender, EventArgs e)
        {
            var simetrias = new SimetriasFrm(analizador.GruposPartidos[grupoPantalla], this);
            simetrias.ShowDialog();
        }		
        private void btnSimetriasII_Click(object sender, EventArgs e)
        {
            var diferenciasFrm = new DiferenciasFrm(analizador.GruposPartidos[grupoPantalla], this);
            diferenciasFrm.ShowDialog();
        }

        void BtnTolGrupoClick(object sender, EventArgs e)
		{
			ControladorTol ctrTol = analizador.GruposPartidos[ grupoPantalla ].ControladorTolerancias;
			var ctrlTolFrm = new ControlTolFrm( ctrTol );
			ctrlTolFrm.ShowDialog();
		}		
		private void btnIfThen_Click(object sender, EventArgs e)
		{
			var f = new IfThenFrm(this);
			f.ShowDialog();
			PonerColorBotonIfThen();
		}		
		protected void PonerColorBotonIfThen()
		{
			if( analizador.IfThen!=null)
			{
				if( analizador.IfThen.EsVacio)
				{
					SetBotonEstado(btnIfThen, BotonEstado.Neutro);
					checkIfThen.Enabled = false;
					checkIfThen.Estado=CtrSemaforo.NombreEstado.Neutro;
				}
				else
				{
					if(analizador.IfThen.EsActivo)
					{
						SetBotonEstado(btnIfThen, BotonEstado.Activo);
						checkIfThen.Enabled = true;
						checkIfThen.Estado=CtrSemaforo.NombreEstado.Verde;
					}
					else
					{
						SetBotonEstado(btnIfThen, BotonEstado.Error);
						checkIfThen.Enabled = true;
						checkIfThen.Estado=CtrSemaforo.NombreEstado.Rojo;
					}
				}
			}
			else
			{
				SetBotonEstado(btnIfThen, BotonEstado.Neutro);
				checkIfThen.Enabled = false;
				checkIfThen.Estado=CtrSemaforo.NombreEstado.Neutro;
			}
		}			
				
		private void checkNoVariantes_BotonPulsado(object sender, EventArgs e)
		{
            EstablecerEstadoDeActivacionDeBoton(checkNoVariantes, Filtro.NoVariantes);
		}

		private void checkPesosNum_BotonPulsado(object sender, EventArgs e)
		{
            EstablecerEstadoDeActivacionDeBoton(checkPesosNum, Filtro.PesosNumericos);
		}

		private void checkSigSeguidos_BotonPulsado(object sender, EventArgs e)
		{
            EstablecerEstadoDeActivacionDeBoton(checkSigSeguidos, Filtro.SignosSeguidos);
		}

		private void checkValoracion_BotonPulsado(object sender, EventArgs e)
		{
            EstablecerEstadoDeActivacionDeBoton(checkValoracion, Filtro.ValoracionSignos);
		}

		private void checkDibujos_BotonPulsado(object sender, EventArgs e)
		{
            EstablecerEstadoDeActivacionDeBoton(checkDibujos, Filtro.Dibujos);
		}

		private void checkCP_BotonPulsado(object sender, EventArgs e)
		{
            EstablecerEstadoDeActivacionDeBoton(checkCP, Filtro.ColProbables);
		}

		private void checkInterrupciones_BotonPulsado(object sender, EventArgs e)
		{
            EstablecerEstadoDeActivacionDeBoton(checkInterrupciones, Filtro.NoInterrupciones);
		}

		private void checkDistancias_BotonPulsado(object sender, EventArgs e)
		{
            EstablecerEstadoDeActivacionDeBoton(checkDistancias, Filtro.Distancias);
	    }

		private void checkGruposEquipos_BotonPulsado(object sender, EventArgs e)
		{
            EstablecerEstadoDeActivacionDeBoton(checkGruposEquipos, Filtro.GruposEquipos);
		}

		private void checkContactos_BotonPulsado(object sender, EventArgs e)
		{
            EstablecerEstadoDeActivacionDeBoton(checkContactos, Filtro.Contactos);
		}

		private void checkFormatos_BotonPulsado(object sender, EventArgs e)
		{
            EstablecerEstadoDeActivacionDeBoton(checkFormatos, Filtro.FormatosSignos);
		}

        private void chkFormatos123_BotonPulsado(object sender, EventArgs e)
        {
            EstablecerEstadoDeActivacionDeBoton(chkFormatos123, Filtro.Formatos123);
        }

        private void EstablecerEstadoDeActivacionDeBoton(CtrSemaforo controlSemaforo, Filtro filtroEnum)
        {
            Grupo grupo = analizador.GruposPartidos[grupoPantalla];

            IFiltro filtro = grupo.GetFiltro(filtroEnum.ToString());

            if (controlSemaforo.Estado == CtrSemaforo.NombreEstado.Verde)
            {
                filtro.IsActive = true;
            }
            else
            {
                filtro.IsActive = false;
            }

            ActualizaGrupoPantalla();
        }

        private void chkSimetrias_BotonPulsado(object sender, EventArgs e)
        {
            EstablecerEstadoDeActivacionDeBoton(checkSimetrias, Filtro.Simetrias);
        }

        private void chkSimetriasII_BotonPulsado(object sender, EventArgs e)
        {
            EstablecerEstadoDeActivacionDeBoton(chkDiferencias, Filtro.Diferencias);
        }
		
		private void checkIfThen_BotonPulsado(object sender, EventArgs e)
		{
			if( checkIfThen.Estado==CtrSemaforo.NombreEstado.Verde )
			{
				SetBotonEstado(btnIfThen, BotonEstado.Activo);
				analizador.IfThen.EsActivo=true;
			}
			else if( checkIfThen.Estado==CtrSemaforo.NombreEstado.Rojo )
			{
				SetBotonEstado(btnIfThen, BotonEstado.Error);
				analizador.IfThen.EsActivo=false;
			}
			else
				SetBotonEstado(btnIfThen, BotonEstado.Neutro);
			ActualizaGrupoPantalla();
		}		
		
		#endregion

		#region Zona Pronostico
		
	    void BtnGrupoPrevClick(object sender, EventArgs e)
		{
            if (pronosticos.NumPartidosActivos == 0)
            {
                BorraGrupoEnPantalla();
               // CambiaGrupo(grupoPantalla - 1);
            }
            else
            {
                CambiaGrupoSeleccionado(grupoPantalla - 1);
            }
		}		
		private void btnGrupoInicio_Click(object sender, EventArgs e)
		{
			CambiaGrupoSeleccionado( 0 );
		}
		private void btnGrupoPrevM_Click(object sender, EventArgs e)
		{
			CambiaGrupoSeleccionado( grupoPantalla - VariablesGlobales.Desplazamiento );
		}
		private void btnGrupoSiguienteM_Click(object sender, EventArgs e)
		{
			CambiaGrupoSeleccionado( grupoPantalla + VariablesGlobales.Desplazamiento );
		}
		private void btnGrupoFin_Click(object sender, EventArgs e)
		{
			CambiaGrupoSeleccionado( analizador.GruposPartidos.Count-1 );
		}		
		void BtnGrupoSiguienteClick(object sender, EventArgs e)
		{
            //Si no es el boleto base, no podemos permitir  que se desplace al siguiente grupo si no
            //hay partidos activos
            if (grupoPantalla > 0)
            {
                if (pronosticos.NumPartidosActivos > 0)
                {
                    CambiaGrupoSeleccionado(grupoPantalla + 1);
                }
                else
                {
                    MessageBox.Show("No ha especificado ningún partido,\nespecifique al menos un partido para seguir", "Atención");
                }
            }
            else
            {
                CambiaGrupoSeleccionado(grupoPantalla + 1);
            }
			
		}		
		protected void CambiaGrupoSeleccionado( int grupoSeleccionado )
		{
            Grupo grupo = analizador.GruposPartidos[grupoPantalla];

			pronosticos.NombreGrupo=grupo.NombreGrupo;
			// Cambia la selección del grupo
			grupoPantalla = grupoSeleccionado;
			pronosticos.GrupoPantalla = grupoSeleccionado;
			
			//crear grupo si grupo no existe 			
			if( analizador.GruposPartidos.Count-1 < grupoSeleccionado )
			{
				grupo = new Grupo();
				analizador.GruposPartidos.AddGrupo( grupo );				
			}			
			
			if( grupoSeleccionado == 0 )
			{
				btnGrupoPrev.Enabled = false;
				menuGrupo.Visible=false;

                btnAbrirGrupo.Visible = false;
                btnGuardarGrupo.Visible = false;
                btnCopiarGrupo.Visible = false;
                btnPegarGrupo.Visible = false;
                btnBorrarGrupo.Visible = false;

                gbFiltroParcial.Enabled = false;
                gbFiltroGeneral.Enabled = true;
			}
			else
			{
				btnGrupoPrev.Enabled = true;
				menuGrupo.Visible=true;


                if (btnAbrirGrupo.Visible == false)
                {
                    btnAbrirGrupo.Visible = true;
                }
                if (btnGuardarGrupo.Visible == false)
                {
                    btnGuardarGrupo.Visible = true;
                }
                if (btnCopiarGrupo.Visible == false)
                {
                    btnCopiarGrupo.Visible = true;
                } 
                if (btnPegarGrupo.Visible == false)
                {
                    btnPegarGrupo.Visible = true;
                }
                if (btnBorrarGrupo.Visible == false)
                {
                    btnBorrarGrupo.Visible = true;
                }
                //Deshabilitar el Filtro General
                gbFiltroGeneral.Enabled = false;

                //Mostrar el Filtro Parcial
                gbFiltroParcial.Enabled = true;

				// Si es el último grupo, deshabilitamos la opción de insertar
				if(analizador.GruposPartidos.Count==grupoSeleccionado+1)
				{
					menuInsertarGrupo.Enabled=false;
					btnInsertarGrupo.Enabled=false;
				}
				else
				{
					menuInsertarGrupo.Enabled=true;
					btnInsertarGrupo.Enabled=true;
				}
				// Comprueba si el botón pegar es habilitable
                FormulariosHelper mf = new FormulariosHelper();
				if(mf.ExisteFicheroTemporal("tmp.grupos"))
				{
					btnPegarGrupo.Enabled=true;
					mPegarGrupo.Enabled=true;
				}
				else
				{
					btnPegarGrupo.Enabled=false;
					mPegarGrupo.Enabled=false;
				}
			}
			// Si no hay partidos activos no se pueden poner condiciones
			pronosticos.ComprobarPartidosActivos();
			ActualizaGrupoPantalla();
			// Comprueba los botones de desplazamiento
			ActivarBotones(grupoSeleccionado);
		}		
		private void ActivarBotones( int noGrupo)
		{
			//activa/desactiva los botones de movimiento
			// Primero y último
			if( analizador.GruposPartidos.Count<=1 )
			{
				btnGrupoInicio.Enabled = false;
				btnGrupoFin.Enabled = false;
			}
			else
			{
				btnGrupoInicio.Enabled = true;
				btnGrupoFin.Enabled = true;
			}
			// Anterior
			if( noGrupo == 0 )
			{
				btnGrupoPrev.Enabled = false;
				btnGrupoInicio.Enabled = false;
			}
			else
			{
				btnGrupoPrev.Enabled = true;
				btnGrupoInicio.Enabled = true;
			}
			// 3 atrás
			if( noGrupo < (VariablesGlobales.Desplazamiento-1) || analizador.GruposPartidos.Count<VariablesGlobales.Desplazamiento)
				btnGrupoPrevM.Enabled = false;
			else
				btnGrupoPrevM.Enabled = true;
			// 3 delante
			if( noGrupo > (analizador.GruposPartidos.Count-VariablesGlobales.Desplazamiento-1) || analizador.GruposPartidos.Count<VariablesGlobales.Desplazamiento)
				btnGrupoSiguienteM.Enabled = false;
			else
				btnGrupoSiguienteM.Enabled = true;

			if( noGrupo==(analizador.GruposPartidos.Count-1) )
				btnGrupoFin.Enabled = false;
			else
				btnGrupoFin.Enabled = true;
		}		
		protected void CambiaGrupo( int grupoSeleccionado )
		{
			// Cambia la selección del grupo
			grupoPantalla = grupoSeleccionado;
			pronosticos.GrupoPantalla = grupoSeleccionado;
		}
		private void HabilitarBoton(object sender, EventArgs e)
		{
            FormulariosHelper f = new FormulariosHelper();
			f.CambiarFondoBoton((Button) sender);
		}
		
	    void BtnControlGruposClick(object sender, EventArgs e)
		{
	        ControlGruposFrm ctrlGruposFrm = new ControlGruposFrm( analizador.CtrlGrupos );
			ctrlGruposFrm.ShowDialog();
			ActualizaGrupoPantalla();
		}		

		// Este método se usa cuando creamos un nuevo grupo a través de código
		// para poner todos los partidos como activos.
		public void ActualizarGruposPronostico()
		{
			bool[] partidosGrupo = new bool[VariablesGlobales.NumeroPartidos];
			for(int i=0;i<partidosGrupo.Length;i++)
			{
				partidosGrupo[i] = true;
			}
			pronosticos.grupoPronosticos.Add( partidosGrupo );
		}		

		#endregion

		#region Zona Filtros, Banner y Notificaciones

		void BtnAddFiltroColsClick(object sender, EventArgs e)
		{
			Button btn = (Button)sender;
			
			if(btn.Tag.ToString() == "+")
			{
				OpenFileDialog abreFiltroDialog = new OpenFileDialog();
				abreFiltroDialog.InitialDirectory = "Filtros\\" ;
				abreFiltroDialog.Filter = "Filtros(*.txt)|*.txt|Todos los archivos (*.*)|*.*" ;
					
				if(abreFiltroDialog.ShowDialog() == DialogResult.OK)
				{
                    ActivaFiltroColumnas(abreFiltroDialog.FileName);
                    IArchivoColumnas cols = new ArchivoColumnasTexto(abreFiltroDialog.FileName);
                    toolTip1.SetToolTip(lblNombreFiltro, cols.ObtenNumCols().ToString("#,##0;0") + " columnas.");
                    btn.Tag = "-";
                    btn.Image = imgCerrar.Image;
				}
			}
			else
			{
				DesactivarFiltroColumnas();
				toolTip1.SetToolTip(lblNombreFiltro,"");
				btn.Tag = "+";
				btn.Image=imgAbrir.Image;
			}
		}				
		protected void ActivaFiltroColumnas(string archivoFiltro)
		{
			archivoFiltroCols = archivoFiltro;

            lblMenosColumnas.Visible = false;
            txtCompletarCon.Visible = false;

            //quitar extension. ej hola.txt -> hola
            string temp = Path.GetFileNameWithoutExtension(archivoFiltroCols);
            lblNombreFiltro.Text = temp;

            if (File.Exists(archivoFiltroCols))
            {		    	
			    chkFiltroCols.Enabled = true;
			    chkFiltroCols.Estado=CtrSemaforo.NombreEstado.Verde;
			    btnAddFiltroCols.Image=imgCerrar.Image;
			    btnAddFiltroCols.Tag="-";
                            
                IArchivoColumnas cols = new ArchivoColumnasTexto(archivoFiltroCols);
                int signos = cols.ObtenNumSignos();
                if (signos < VariablesGlobales.NumeroPartidos)
                {
                    //Mostrar los controles para completar las columnas
                    lblMenosColumnas.Visible = true;
                    lblMenosColumnas.Text = "El filtro tiene menos de " + VariablesGlobales.NumeroPartidos+ " signos. Completar con:";
                    txtCompletarCon.Visible = true;
                    txtCompletarCon.MaxLength = VariablesGlobales.NumeroPartidos - signos;
                    txtCompletarCon.Width = txtCompletarCon.MaxLength * 15;
                }                
            }
            else
            {
                lblNombreFiltro.Text += " ERROR: Archivo no encontrado";
                chkFiltroCols.Enabled = false;
                chkFiltroCols.Estado = CtrSemaforo.NombreEstado.Rojo;
                btnAddFiltroCols.Image = imgCerrar.Image;
                btnAddFiltroCols.Tag = "-";

                MessageBox.Show("El filtro especificado no existe o se ha cambiado de carpeta","Error al cargar filtro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
		}
		protected void DesactivarFiltroColumnas()
		{
			archivoFiltroCols = "";
			lblNombreFiltro.Text = "";
			chkFiltroCols.Enabled = false;
			chkFiltroCols.Estado=CtrSemaforo.NombreEstado.Neutro;
            txtCompletarCon.Visible = false;
            lblMenosColumnas.Visible = false;
		}

        private void btnAbreFiltroParcial_Click(object sender, EventArgs e)
        {
            //El filtro que se elija aquí se aplicará sólo al grupo en pantalla
            Button btn = (Button)sender;

            if (btn.Tag.ToString() == "+")
            {
                OpenFileDialog abreFiltroDialog = new OpenFileDialog();
                abreFiltroDialog.InitialDirectory = "Filtros\\";
                abreFiltroDialog.Filter = "Filtros(*.txt)|*.txt|Todos los archivos (*.*)|*.*";

                if (abreFiltroDialog.ShowDialog() == DialogResult.OK)
                {
                    if (ActivaFiltroColumnasParcial(abreFiltroDialog.FileName))
                    {
                        IArchivoColumnas cols = new ArchivoColumnasTexto(abreFiltroDialog.FileName);
                        toolTip1.SetToolTip(lblNombreFiltro, cols.ObtenNumCols().ToString("#,##0;0") + " columnas.");
                        btn.Tag = "-";
                        btn.Image = imgCerrar.Image;

                        //En este punto indicar al grupo cual es su filtro
                        Grupo grupo = analizador.GruposPartidos[grupoPantalla];
                        grupo.ArchivoFiltroGrupo = abreFiltroDialog.FileName;

                    }
                }
            }
            else
            {
                DesactivarFiltroColumnasParcial();
                toolTip1.SetToolTip(lblNombreFiltroParcial, "");
            }
        }
		
        private void pBQuinielista_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.quinielista.com/home.asp?r=1275");
        }		

        private void pctNotificaciones_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Notifications system disabled for performance.", "Notifications", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        // Notification system removed for performance optimization
		#endregion
	}
}
