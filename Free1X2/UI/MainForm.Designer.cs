using System.Windows.Forms;
using Free1X2.UI.Controls;

namespace Free1X2.UI
{
	public partial class MainForm
	{
		#region Windows Form Designer generated code
		private ToolStripMenuItem mAbrirComb;
        private ToolStripMenuItem menuItem3;
        private ToolStripMenuItem menuItem2;
        private ToolStripMenuItem menuItem7;
        private ToolStripMenuItem menuItem6;
        private ToolStripMenuItem menuItem5;
        private ToolStripMenuItem menuCombinacion;
        private ToolStripMenuItem menuArchivo;
        private ToolStripMenuItem menuItem9;
        private ToolStripMenuItem menuItem8;
        private ToolStripMenuItem menuUtilidades;
        private ToolStripMenuItem menuItem10;
        private ToolStripMenuItem menuItem11;
        private ToolStripMenuItem menuItem12;
        private ToolStripMenuItem menuItem13;
        private ToolStripMenuItem menuItem14;
        private ToolStripMenuItem menuItem15;
        private ToolStripMenuItem menuItem17;
        private ToolStripMenuItem menuItem18;
        private ToolStripMenuItem menuItem19;
        private ToolStripMenuItem menuItem21;
        private ToolStripMenuItem menuItem23;
        private ToolStripMenuItem menuItem24;
        private ToolStripMenuItem menuItem25;
        private ToolStripMenuItem menuItem26;
        private ToolStripMenuItem menuItem28;
        private ToolStripMenuItem menuItem31;
        private ToolStripMenuItem menuItem32;
        private ToolStripMenuItem menuItem33;
        private ToolStripMenuItem menuItem34;
        private ToolStripMenuItem menuItem35;
        private ToolStripMenuItem menuItem36;
        private ToolStripMenuItem menuItem38;
        private ToolStripMenuItem mFiltroAidomnou;
        private ToolStripMenuItem mFiltroPim;
        private ToolStripMenuItem menuFree1X2;
        private ToolStripMenuItem menuOperaciones;
        private ToolStripMenuItem menuFiltros;
        private ToolStripMenuItem menuItem42;
        private ToolStripMenuItem menuItem43;
        private ToolStripMenuItem menuItem44;
        private ToolStripMenuItem menuItem45;
        private ToolStripMenuItem menuItem46;
        private ToolStripMenuItem menuItem47;
        private ToolStripMenuItem menuItem48;
        private ToolStripMenuItem menuItem49;
        private ToolStripMenuItem menuItem22;
        private ToolStripMenuItem menuItem27;
        private ToolStripMenuItem menuItem37;
        private ToolStripMenuItem menuGrupo;
        private ToolStripMenuItem menuInsertarGrupo;
        private ToolStripMenuItem menuItem20;
        private ToolStripMenuItem menuItem30;
        private ToolStripMenuItem mPegarGrupo;
        private ToolStripMenuItem menuItem29;
        private ToolStripMenuItem menuItem16;
        private ToolStripMenuItem menuItem50;
        private ToolStripMenuItem menuItem51;
        private ToolStripMenuItem menuItem52;
        private ToolStripMenuItem menuItem53;
        private ToolStripMenuItem menuItem54;
        private ToolStripMenuItem menuItem55;
        private ToolStripMenuItem menuItem56;
        private ToolStripMenuItem MnuReduccionesPerfectas;
        private ToolStripMenuItem menuDepLineal;
        private GroupBox gbFiltroGeneral;
        private GroupBox groupBox2;
        private Button btnTolGrupo;
        private Button btnPesosNum;
        private Button btnControlGrupos;
        private Label lblNombreFiltro;
        private Button btnSignosSeguidos;
        private Button btnGrupoPrev;
        private Button btnDibujos;
        private Button btnGrupoSiguiente;
        private Button btnAddFiltroCols;
        private Button btnNoVariantes;
        private Button btnCP;
        private Button btnValoracion;
        private Button btnIterrupciones;
        private Button btnDistancias;
        private Button btnGruposEquipos;
        private Button btnContactos;
        private Button btnFormatos;
        private ImageList ilUtilidades;
        private ImageList ilArchivo;
        private ImageList ilFree;
        private ImageList ilCombinacion;
        private ImageList ilFiltros;
        private ImageList ilOperaciones;
        private PictureBox imgAbrir;
        private PictureBox imgCerrar;
        private CtrSemaforo chkFiltroCols;
        private CtrSemaforo checkDistancias;
        private CtrSemaforo checkInterrupciones;
        private CtrSemaforo checkContactos;
        private CtrSemaforo checkGruposEquipos;
        private CtrSemaforo checkFormatos;
        private CtrSemaforo checkCP;
        private CtrSemaforo checkDibujos;
        private CtrSemaforo checkValoracion;
        private CtrSemaforo checkSigSeguidos;
        private CtrSemaforo checkPesosNum;
        private CtrSemaforo checkNoVariantes;
        private ImageList ilGrupos;
        private Button btnGrupoPrevM;
        private Button btnGrupoInicio;
        private Button btnGrupoFin;
        private Button btnGrupoSiguienteM;
        private ToolTip toolTip1;
        private Button btnIfThen;
        private CtrSemaforo checkIfThen;
        private ToolStripContainer toolStripContainer1;
        private ToolStrip tsFree;
        private ToolStripButton bSalir;
        private ToolStripButton bConfig;
        private ToolStripButton bAcercaDe;
        private ToolStrip tsUtilidades;
        private ToolStripButton bSubeCategoria;
        private ToolStripButton bModificadorPct;
        private ToolStripButton bGeneradorCPs;
        private ToolStripButton bDiferenciasColumnas;
        private ToolStripButton bProbabilidad;
        private ToolStripButton bSelectorJuanM;
        private ToolStripButton bSelectorMarioSan;
        private ToolStripButton bRentabilidad;
        private ToolStripButton bColumnasGEPT;
        private ToolStripButton bTramificar;
        private ToolStripButton bPremiadas;
        private ToolStripButton bEstimacion;
        private ToolStripButton bBancoPruebas;
        private ToolStripButton bImportExport;
        private ToolStripButton bDependenciaLineal;
        private ToolStrip tsArchivo;
        private ToolStripButton bAbrirEquipos;
        private ToolStripButton bGuardarEquipos;
        private ToolStripButton bNuevo;
        private ToolStripButton bAbrirCombinacion;
        private ToolStripButton bGuardarCombinacion;
        private ToolStripButton bGuardarCombinacionComo;
        private ToolStrip tsCombinacion;
        private ToolStripButton bCalcular;
        private ToolStripButton bCalcularM;
        private ToolStripButton bVerBoletos;
        private ToolStripButton bImprimirBoletos;
        private ToolStripButton bReducir;
        private ToolStripButton bEscrutinio;
        private ToolStripButton bAnalisisColumnas;
        private ToolStripButton bAnalisisFallos;
        private ToolStripButton bAnalisisGrafico;
        private ToolStripButton bAnalisisSignos;
        private ToolStripButton bProbabilidades;
        private ToolStripButton bEstadisticas;
        private ToolStrip tsOperaciones;
        private ToolStripButton bAlgebra;
        private ToolStripButton bTransposición;
        private ToolStripButton bMultiplicacion;
        private ToolStripButton bFraccionador;
        private ToolStripButton bRotacion;
        private ToolStripButton bAnalisisGrupos;
        private ToolStripButton bRedPerfectas;
        private ToolStrip tsFiltros;
        private ToolStripButton bCombinarFiltros;
        private ToolStripButton bDiferenciasFiltros;
        private ToolStripButton bFiltroCoincidencias;
        private ToolStripButton bFiltroAidomnou;
        private ToolStripButton bFiltroPim;
        private ToolStripButton toolStripButton3;
        private ToolStripButton toolStripButton4;
        private ToolStripButton toolStripButton5;
        private ToolStripButton bAyuda;
        private ToolStripButton bBorrarTemporales;
        private ToolStripButton bP15;
        private CtrSemaforo chkFormatos123;
        private Button btnFormatos123;
        private CtrSemaforo ctrSemaforo1;
        private ToolStripMenuItem escrutarCombinacionesToolStripMenuItem;
        private ToolStripButton bEscrutarComb;
        private CtrSemaforo checkSimetrias;
        private Button btnSimetrias;
        private ToolStripMenuItem personalizarToolStripMenuItem;
        private ToolStripMenuItem filtrosToolStripMenuItem;
        private ToolStripMenuItem free1X2ToolStripMenuItem;
        private ToolStripMenuItem operacionesToolStripMenuItem;
        private ToolStripMenuItem utilidadesToolStripMenuItem;
        private ToolStripMenuItem combinacionToolStripMenuItem;
        private ToolStripMenuItem archivoToolStripMenuItem;
        private ToolStripMenuItem menuVer;
        private ToolStripMenuItem listadoDeCondicionesToolStripMenuItem;
        private ToolStripMenuItem comprobarActualizacionesToolStripMenuItem;
        private ToolStripButton bConfAnalisis;
        private ToolStripMenuItem configurarAnálisisToolStripMenuItem;
        private ToolStripMenuItem compresorToolStripMenuItem;
        private ToolStripMenuItem estuColToolStripMenuItem;
        private ToolStripMenuItem borrarInformesDeErrorToolStripMenuItem;
        private ToolStripMenuItem gestiónDeEquiposToolStripMenuItem;
        private ToolStripButton bObtenerBoletos;
        private ToolStripButton bBorrarInformes;
        private ToolStripButton bGestorEquipos;
        private ToolStripMenuItem verBoletosEnEditorDeTextoToolStripMenuItem;
        private Label lblMenosColumnas;
        private TextBox txtCompletarCon;
        private GroupBox gbFiltroParcial;
        private PictureBox pictureBox1;
        private PictureBox pictureBox2;
        private CtrSemaforo chkFiltroColsParcial;
        private Label lblNombreFiltroParcial;
        private Button btnAbreFiltroParcial;
        private Label label2;
        private ToolStripContainer toolStripContainer3;
        private ToolStripContainer toolStripContainer4;
        private Button btnBorrarGrupo;
        private Button btnInsertarGrupo;
        private Button btnPegarGrupo;
        private Button btnCopiarGrupo;
        private Button btnGuardarGrupo;
        private Button btnAbrirGrupo;
        private CtrSemaforo chkDiferencias;
        private Button btnDiferencias;
        private PictureBox pBQuinielista;
        private ToolStripMenuItem obtenerBoletosOnlineToolStripMenuItem;
        private MenuStrip mainMenu;
        private ImageList imgListNotificaciones;
        private PictureBox pctNotificaciones;
        public GroupBox groupBox;
        public Pronosticos pronosticos;

		void InitializeComponent() 
		{
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.lblNombreFiltro = new System.Windows.Forms.Label();
            this.groupBox = new System.Windows.Forms.GroupBox();
            this.btnDiferencias = new System.Windows.Forms.Button();
            this.btnSimetrias = new System.Windows.Forms.Button();
            this.btnFormatos123 = new System.Windows.Forms.Button();
            this.btnIfThen = new System.Windows.Forms.Button();
            this.btnFormatos = new System.Windows.Forms.Button();
            this.btnContactos = new System.Windows.Forms.Button();
            this.btnGruposEquipos = new System.Windows.Forms.Button();
            this.btnDistancias = new System.Windows.Forms.Button();
            this.btnIterrupciones = new System.Windows.Forms.Button();
            this.btnValoracion = new System.Windows.Forms.Button();
            this.btnTolGrupo = new System.Windows.Forms.Button();
            this.btnPesosNum = new System.Windows.Forms.Button();
            this.btnCP = new System.Windows.Forms.Button();
            this.btnDibujos = new System.Windows.Forms.Button();
            this.btnSignosSeguidos = new System.Windows.Forms.Button();
            this.btnNoVariantes = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnBorrarGrupo = new System.Windows.Forms.Button();
            this.btnInsertarGrupo = new System.Windows.Forms.Button();
            this.btnPegarGrupo = new System.Windows.Forms.Button();
            this.btnCopiarGrupo = new System.Windows.Forms.Button();
            this.btnGuardarGrupo = new System.Windows.Forms.Button();
            this.btnAbrirGrupo = new System.Windows.Forms.Button();
            this.btnGrupoPrevM = new System.Windows.Forms.Button();
            this.btnGrupoInicio = new System.Windows.Forms.Button();
            this.btnGrupoFin = new System.Windows.Forms.Button();
            this.btnGrupoSiguienteM = new System.Windows.Forms.Button();
            this.btnControlGrupos = new System.Windows.Forms.Button();
            this.btnGrupoSiguiente = new System.Windows.Forms.Button();
            this.btnGrupoPrev = new System.Windows.Forms.Button();
            this.gbFiltroGeneral = new System.Windows.Forms.GroupBox();
            this.lblMenosColumnas = new System.Windows.Forms.Label();
            this.txtCompletarCon = new System.Windows.Forms.TextBox();
            this.imgCerrar = new System.Windows.Forms.PictureBox();
            this.imgAbrir = new System.Windows.Forms.PictureBox();
            this.btnAddFiltroCols = new System.Windows.Forms.Button();
            this.ilUtilidades = new System.Windows.Forms.ImageList(this.components);
            this.ilArchivo = new System.Windows.Forms.ImageList(this.components);
            this.ilFree = new System.Windows.Forms.ImageList(this.components);
            this.ilCombinacion = new System.Windows.Forms.ImageList(this.components);
            this.ilFiltros = new System.Windows.Forms.ImageList(this.components);
            this.ilOperaciones = new System.Windows.Forms.ImageList(this.components);
            this.ilGrupos = new System.Windows.Forms.ImageList(this.components);
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.toolStripContainer1 = new System.Windows.Forms.ToolStripContainer();
            this.pBQuinielista = new System.Windows.Forms.PictureBox();
            this.gbFiltroParcial = new System.Windows.Forms.GroupBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.lblNombreFiltroParcial = new System.Windows.Forms.Label();
            this.btnAbreFiltroParcial = new System.Windows.Forms.Button();
            this.mainMenu = new System.Windows.Forms.MenuStrip();
            this.menuFree1X2 = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItem42 = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItem43 = new System.Windows.Forms.ToolStripMenuItem();
            this.configurarAnálisisToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItem44 = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItem53 = new System.Windows.Forms.ToolStripMenuItem();
            this.comprobarActualizacionesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuArchivo = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItem8 = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItem9 = new System.Windows.Forms.ToolStripMenuItem();
            this.obtenerBoletosOnlineToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItem3 = new System.Windows.Forms.ToolStripMenuItem();
            this.mAbrirComb = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItem7 = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItem52 = new System.Windows.Forms.ToolStripMenuItem();
            this.borrarInformesDeErrorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.gestiónDeEquiposToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuVer = new System.Windows.Forms.ToolStripMenuItem();
            this.personalizarToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.filtrosToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.free1X2ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.operacionesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.utilidadesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.combinacionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.archivoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.listadoDeCondicionesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuCombinacion = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItem45 = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItem16 = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItem50 = new System.Windows.Forms.ToolStripMenuItem();
            this.verBoletosEnEditorDeTextoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItem29 = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItem46 = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItem47 = new System.Windows.Forms.ToolStripMenuItem();
            this.escrutarCombinacionesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItem56 = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItem20 = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItem15 = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItem28 = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItem48 = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItem49 = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItem51 = new System.Windows.Forms.ToolStripMenuItem();
            this.menuGrupo = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItem22 = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItem30 = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItem27 = new System.Windows.Forms.ToolStripMenuItem();
            this.mPegarGrupo = new System.Windows.Forms.ToolStripMenuItem();
            this.menuInsertarGrupo = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItem37 = new System.Windows.Forms.ToolStripMenuItem();
            this.menuFiltros = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItem13 = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItem17 = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItem23 = new System.Windows.Forms.ToolStripMenuItem();
            this.mFiltroAidomnou = new System.Windows.Forms.ToolStripMenuItem();
            this.mFiltroPim = new System.Windows.Forms.ToolStripMenuItem();
            this.menuOperaciones = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItem6 = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItem31 = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItem32 = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItem33 = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItem34 = new System.Windows.Forms.ToolStripMenuItem();
            this.menuUtilidades = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItem10 = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItem11 = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItem12 = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItem14 = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItem18 = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItem36 = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItem19 = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItem21 = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItem24 = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItem25 = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItem26 = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItem35 = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItem38 = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItem55 = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItem54 = new System.Windows.Forms.ToolStripMenuItem();
            this.MnuReduccionesPerfectas = new System.Windows.Forms.ToolStripMenuItem();
            this.menuDepLineal = new System.Windows.Forms.ToolStripMenuItem();
            this.compresorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.estuColToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tsFree = new System.Windows.Forms.ToolStrip();
            this.bSalir = new System.Windows.Forms.ToolStripButton();
            this.bConfig = new System.Windows.Forms.ToolStripButton();
            this.bConfAnalisis = new System.Windows.Forms.ToolStripButton();
            this.bAyuda = new System.Windows.Forms.ToolStripButton();
            this.bAcercaDe = new System.Windows.Forms.ToolStripButton();
            this.tsArchivo = new System.Windows.Forms.ToolStrip();
            this.bGuardarEquipos = new System.Windows.Forms.ToolStripButton();
            this.bNuevo = new System.Windows.Forms.ToolStripButton();
            this.bObtenerBoletos = new System.Windows.Forms.ToolStripButton();
            this.bAbrirCombinacion = new System.Windows.Forms.ToolStripButton();
            this.bGuardarCombinacion = new System.Windows.Forms.ToolStripButton();
            this.bGuardarCombinacionComo = new System.Windows.Forms.ToolStripButton();
            this.bBorrarTemporales = new System.Windows.Forms.ToolStripButton();
            this.bAbrirEquipos = new System.Windows.Forms.ToolStripButton();
            this.bBorrarInformes = new System.Windows.Forms.ToolStripButton();
            this.bGestorEquipos = new System.Windows.Forms.ToolStripButton();
            this.tsOperaciones = new System.Windows.Forms.ToolStrip();
            this.bAlgebra = new System.Windows.Forms.ToolStripButton();
            this.bTransposición = new System.Windows.Forms.ToolStripButton();
            this.bMultiplicacion = new System.Windows.Forms.ToolStripButton();
            this.bFraccionador = new System.Windows.Forms.ToolStripButton();
            this.bRotacion = new System.Windows.Forms.ToolStripButton();
            this.tsUtilidades = new System.Windows.Forms.ToolStrip();
            this.bSubeCategoria = new System.Windows.Forms.ToolStripButton();
            this.bModificadorPct = new System.Windows.Forms.ToolStripButton();
            this.bGeneradorCPs = new System.Windows.Forms.ToolStripButton();
            this.bDiferenciasColumnas = new System.Windows.Forms.ToolStripButton();
            this.bProbabilidad = new System.Windows.Forms.ToolStripButton();
            this.bSelectorJuanM = new System.Windows.Forms.ToolStripButton();
            this.bSelectorMarioSan = new System.Windows.Forms.ToolStripButton();
            this.bRentabilidad = new System.Windows.Forms.ToolStripButton();
            this.bColumnasGEPT = new System.Windows.Forms.ToolStripButton();
            this.bTramificar = new System.Windows.Forms.ToolStripButton();
            this.bPremiadas = new System.Windows.Forms.ToolStripButton();
            this.bEstimacion = new System.Windows.Forms.ToolStripButton();
            this.bBancoPruebas = new System.Windows.Forms.ToolStripButton();
            this.bImportExport = new System.Windows.Forms.ToolStripButton();
            this.bAnalisisGrupos = new System.Windows.Forms.ToolStripButton();
            this.bRedPerfectas = new System.Windows.Forms.ToolStripButton();
            this.bDependenciaLineal = new System.Windows.Forms.ToolStripButton();
            this.tsCombinacion = new System.Windows.Forms.ToolStrip();
            this.bCalcular = new System.Windows.Forms.ToolStripButton();
            this.bCalcularM = new System.Windows.Forms.ToolStripButton();
            this.bVerBoletos = new System.Windows.Forms.ToolStripButton();
            this.bImprimirBoletos = new System.Windows.Forms.ToolStripButton();
            this.bReducir = new System.Windows.Forms.ToolStripButton();
            this.bEscrutinio = new System.Windows.Forms.ToolStripButton();
            this.bEscrutarComb = new System.Windows.Forms.ToolStripButton();
            this.bAnalisisColumnas = new System.Windows.Forms.ToolStripButton();
            this.bAnalisisFallos = new System.Windows.Forms.ToolStripButton();
            this.bAnalisisGrafico = new System.Windows.Forms.ToolStripButton();
            this.bAnalisisSignos = new System.Windows.Forms.ToolStripButton();
            this.bProbabilidades = new System.Windows.Forms.ToolStripButton();
            this.bEstadisticas = new System.Windows.Forms.ToolStripButton();
            this.bP15 = new System.Windows.Forms.ToolStripButton();
            this.tsFiltros = new System.Windows.Forms.ToolStrip();
            this.bCombinarFiltros = new System.Windows.Forms.ToolStripButton();
            this.bDiferenciasFiltros = new System.Windows.Forms.ToolStripButton();
            this.bFiltroCoincidencias = new System.Windows.Forms.ToolStripButton();
            this.bFiltroAidomnou = new System.Windows.Forms.ToolStripButton();
            this.bFiltroPim = new System.Windows.Forms.ToolStripButton();
            this.menuItem5 = new System.Windows.Forms.ToolStripMenuItem();
            this.label2 = new System.Windows.Forms.Label();
            this.toolStripContainer3 = new System.Windows.Forms.ToolStripContainer();
            this.toolStripContainer4 = new System.Windows.Forms.ToolStripContainer();
            this.toolStripButton3 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton4 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton5 = new System.Windows.Forms.ToolStripButton();
            this.imgListNotificaciones = new System.Windows.Forms.ImageList(this.components);
            this.chkFiltroColsParcial = new Free1X2.UI.Controls.CtrSemaforo();
            this.chkFiltroCols = new Free1X2.UI.Controls.CtrSemaforo();
            this.pronosticos = new Free1X2.UI.Controls.Pronosticos();
            this.chkDiferencias = new Free1X2.UI.Controls.CtrSemaforo();
            this.checkSimetrias = new Free1X2.UI.Controls.CtrSemaforo();
            this.chkFormatos123 = new Free1X2.UI.Controls.CtrSemaforo();
            this.checkIfThen = new Free1X2.UI.Controls.CtrSemaforo();
            this.checkFormatos = new Free1X2.UI.Controls.CtrSemaforo();
            this.checkContactos = new Free1X2.UI.Controls.CtrSemaforo();
            this.checkGruposEquipos = new Free1X2.UI.Controls.CtrSemaforo();
            this.checkDistancias = new Free1X2.UI.Controls.CtrSemaforo();
            this.checkInterrupciones = new Free1X2.UI.Controls.CtrSemaforo();
            this.checkCP = new Free1X2.UI.Controls.CtrSemaforo();
            this.checkDibujos = new Free1X2.UI.Controls.CtrSemaforo();
            this.checkValoracion = new Free1X2.UI.Controls.CtrSemaforo();
            this.checkSigSeguidos = new Free1X2.UI.Controls.CtrSemaforo();
            this.checkPesosNum = new Free1X2.UI.Controls.CtrSemaforo();
            this.checkNoVariantes = new Free1X2.UI.Controls.CtrSemaforo();
            this.ctrSemaforo1 = new Free1X2.UI.Controls.CtrSemaforo();
            this.pctNotificaciones = new System.Windows.Forms.PictureBox();
            this.groupBox.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.gbFiltroGeneral.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.imgCerrar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.imgAbrir)).BeginInit();
            this.toolStripContainer1.ContentPanel.SuspendLayout();
            this.toolStripContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pBQuinielista)).BeginInit();
            this.gbFiltroParcial.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.mainMenu.SuspendLayout();
            this.tsFree.SuspendLayout();
            this.tsArchivo.SuspendLayout();
            this.tsOperaciones.SuspendLayout();
            this.tsUtilidades.SuspendLayout();
            this.tsCombinacion.SuspendLayout();
            this.tsFiltros.SuspendLayout();
            this.toolStripContainer3.TopToolStripPanel.SuspendLayout();
            this.toolStripContainer3.SuspendLayout();
            this.toolStripContainer4.TopToolStripPanel.SuspendLayout();
            this.toolStripContainer4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pctNotificaciones)).BeginInit();
            this.SuspendLayout();
            // 
            // lblNombreFiltro
            // 
            this.lblNombreFiltro.Font = new System.Drawing.Font("Verdana", 7F);
            this.lblNombreFiltro.ForeColor = System.Drawing.Color.Black;
            this.lblNombreFiltro.Location = new System.Drawing.Point(72, 25);
            this.lblNombreFiltro.Name = "lblNombreFiltro";
            this.lblNombreFiltro.Size = new System.Drawing.Size(230, 16);
            this.lblNombreFiltro.TabIndex = 1;
            this.lblNombreFiltro.Text = "(Selecciona)";
            this.lblNombreFiltro.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // groupBox
            // 
            this.groupBox.BackColor = System.Drawing.Color.Bisque;
            this.groupBox.Controls.Add(this.chkDiferencias);
            this.groupBox.Controls.Add(this.btnDiferencias);
            this.groupBox.Controls.Add(this.checkSimetrias);
            this.groupBox.Controls.Add(this.btnSimetrias);
            this.groupBox.Controls.Add(this.chkFormatos123);
            this.groupBox.Controls.Add(this.btnFormatos123);
            this.groupBox.Controls.Add(this.checkIfThen);
            this.groupBox.Controls.Add(this.btnIfThen);
            this.groupBox.Controls.Add(this.checkFormatos);
            this.groupBox.Controls.Add(this.checkContactos);
            this.groupBox.Controls.Add(this.checkGruposEquipos);
            this.groupBox.Controls.Add(this.checkDistancias);
            this.groupBox.Controls.Add(this.checkInterrupciones);
            this.groupBox.Controls.Add(this.checkCP);
            this.groupBox.Controls.Add(this.checkDibujos);
            this.groupBox.Controls.Add(this.checkValoracion);
            this.groupBox.Controls.Add(this.checkSigSeguidos);
            this.groupBox.Controls.Add(this.checkPesosNum);
            this.groupBox.Controls.Add(this.checkNoVariantes);
            this.groupBox.Controls.Add(this.btnFormatos);
            this.groupBox.Controls.Add(this.btnContactos);
            this.groupBox.Controls.Add(this.btnGruposEquipos);
            this.groupBox.Controls.Add(this.btnDistancias);
            this.groupBox.Controls.Add(this.btnIterrupciones);
            this.groupBox.Controls.Add(this.btnValoracion);
            this.groupBox.Controls.Add(this.btnTolGrupo);
            this.groupBox.Controls.Add(this.btnPesosNum);
            this.groupBox.Controls.Add(this.btnCP);
            this.groupBox.Controls.Add(this.btnDibujos);
            this.groupBox.Controls.Add(this.btnSignosSeguidos);
            this.groupBox.Controls.Add(this.btnNoVariantes);
            this.groupBox.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox.ForeColor = System.Drawing.Color.Maroon;
            this.groupBox.Location = new System.Drawing.Point(417, 17);
            this.groupBox.Name = "groupBox";
            this.groupBox.Size = new System.Drawing.Size(343, 315);
            this.groupBox.TabIndex = 3;
            this.groupBox.TabStop = false;
            this.groupBox.Text = "Condiciones";
            // 
            // btnDiferencias
            // 
            this.btnDiferencias.BackColor = System.Drawing.Color.DarkSalmon;
            this.btnDiferencias.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnDiferencias.Font = new System.Drawing.Font("Verdana", 8F);
            this.btnDiferencias.ForeColor = System.Drawing.Color.Black;
            this.btnDiferencias.Image = ((System.Drawing.Image)(resources.GetObject("btnDiferencias.Image")));
            this.btnDiferencias.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnDiferencias.Location = new System.Drawing.Point(182, 216);
            this.btnDiferencias.Name = "btnDiferencias";
            this.btnDiferencias.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.btnDiferencias.Size = new System.Drawing.Size(145, 28);
            this.btnDiferencias.TabIndex = 39;
            this.btnDiferencias.Text = "Diferencias";
            this.btnDiferencias.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnDiferencias.UseVisualStyleBackColor = false;
            this.btnDiferencias.Click += new System.EventHandler(this.btnSimetriasII_Click);
            // 
            // btnSimetrias
            // 
            this.btnSimetrias.BackColor = System.Drawing.Color.DarkSalmon;
            this.btnSimetrias.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnSimetrias.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSimetrias.ForeColor = System.Drawing.Color.Black;
            this.btnSimetrias.Image = ((System.Drawing.Image)(resources.GetObject("btnSimetrias.Image")));
            this.btnSimetrias.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSimetrias.Location = new System.Drawing.Point(21, 216);
            this.btnSimetrias.Name = "btnSimetrias";
            this.btnSimetrias.Size = new System.Drawing.Size(145, 28);
            this.btnSimetrias.TabIndex = 37;
            this.btnSimetrias.Text = "Simetrías";
            this.btnSimetrias.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnSimetrias.UseVisualStyleBackColor = false;
            this.btnSimetrias.Click += new System.EventHandler(this.btnSimetrias_Click);
            // 
            // btnFormatos123
            // 
            this.btnFormatos123.BackColor = System.Drawing.Color.DarkSalmon;
            this.btnFormatos123.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnFormatos123.Font = new System.Drawing.Font("Verdana", 8F);
            this.btnFormatos123.ForeColor = System.Drawing.Color.Black;
            this.btnFormatos123.Image = ((System.Drawing.Image)(resources.GetObject("btnFormatos123.Image")));
            this.btnFormatos123.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnFormatos123.Location = new System.Drawing.Point(182, 184);
            this.btnFormatos123.Name = "btnFormatos123";
            this.btnFormatos123.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.btnFormatos123.Size = new System.Drawing.Size(145, 28);
            this.btnFormatos123.TabIndex = 35;
            this.btnFormatos123.Text = "Formatos 123";
            this.btnFormatos123.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnFormatos123.UseVisualStyleBackColor = false;
            this.btnFormatos123.Click += new System.EventHandler(this.btnFormatos123_Click);
            // 
            // btnIfThen
            // 
            this.btnIfThen.BackColor = System.Drawing.Color.LightGoldenrodYellow;
            this.btnIfThen.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnIfThen.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnIfThen.ForeColor = System.Drawing.Color.Maroon;
            this.btnIfThen.Image = ((System.Drawing.Image)(resources.GetObject("btnIfThen.Image")));
            this.btnIfThen.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnIfThen.Location = new System.Drawing.Point(182, 264);
            this.btnIfThen.Name = "btnIfThen";
            this.btnIfThen.Size = new System.Drawing.Size(200, 34);
            this.btnIfThen.TabIndex = 33;
            this.btnIfThen.Text = "Condiciones Relacionadas";
            this.btnIfThen.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.btnIfThen.UseVisualStyleBackColor = false;
            this.btnIfThen.Click += new System.EventHandler(this.btnIfThen_Click);
            // 
            // btnFormatos
            // 
            this.btnFormatos.BackColor = System.Drawing.Color.DarkSalmon;
            this.btnFormatos.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnFormatos.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnFormatos.ForeColor = System.Drawing.Color.Black;
            this.btnFormatos.Image = ((System.Drawing.Image)(resources.GetObject("btnFormatos.Image")));
            this.btnFormatos.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnFormatos.Location = new System.Drawing.Point(21, 184);
            this.btnFormatos.Name = "btnFormatos";
            this.btnFormatos.Size = new System.Drawing.Size(145, 28);
            this.btnFormatos.TabIndex = 21;
            this.btnFormatos.Text = "Formatos";
            this.btnFormatos.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnFormatos.UseVisualStyleBackColor = false;
            this.btnFormatos.Click += new System.EventHandler(this.btnFormatos_Click);
            // 
            // btnContactos
            // 
            this.btnContactos.BackColor = System.Drawing.Color.DarkSalmon;
            this.btnContactos.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnContactos.Font = new System.Drawing.Font("Verdana", 8F);
            this.btnContactos.ForeColor = System.Drawing.Color.Black;
            this.btnContactos.Image = ((System.Drawing.Image)(resources.GetObject("btnContactos.Image")));
            this.btnContactos.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnContactos.Location = new System.Drawing.Point(182, 152);
            this.btnContactos.Name = "btnContactos";
            this.btnContactos.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.btnContactos.Size = new System.Drawing.Size(145, 28);
            this.btnContactos.TabIndex = 19;
            this.btnContactos.Text = "Contactos";
            this.btnContactos.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnContactos.UseVisualStyleBackColor = false;
            this.btnContactos.Click += new System.EventHandler(this.btnContactos_Click);
            // 
            // btnGruposEquipos
            // 
            this.btnGruposEquipos.BackColor = System.Drawing.Color.DarkSalmon;
            this.btnGruposEquipos.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnGruposEquipos.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnGruposEquipos.ForeColor = System.Drawing.Color.Black;
            this.btnGruposEquipos.Image = ((System.Drawing.Image)(resources.GetObject("btnGruposEquipos.Image")));
            this.btnGruposEquipos.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnGruposEquipos.Location = new System.Drawing.Point(21, 152);
            this.btnGruposEquipos.Name = "btnGruposEquipos";
            this.btnGruposEquipos.Size = new System.Drawing.Size(145, 28);
            this.btnGruposEquipos.TabIndex = 17;
            this.btnGruposEquipos.Text = "Grupos Equipos";
            this.btnGruposEquipos.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnGruposEquipos.UseVisualStyleBackColor = false;
            this.btnGruposEquipos.Click += new System.EventHandler(this.btnGruposEquipos_Click);
            // 
            // btnDistancias
            // 
            this.btnDistancias.BackColor = System.Drawing.Color.DarkSalmon;
            this.btnDistancias.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnDistancias.Font = new System.Drawing.Font("Verdana", 8F);
            this.btnDistancias.ForeColor = System.Drawing.Color.Black;
            this.btnDistancias.Image = ((System.Drawing.Image)(resources.GetObject("btnDistancias.Image")));
            this.btnDistancias.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnDistancias.Location = new System.Drawing.Point(182, 120);
            this.btnDistancias.Name = "btnDistancias";
            this.btnDistancias.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.btnDistancias.Size = new System.Drawing.Size(145, 28);
            this.btnDistancias.TabIndex = 15;
            this.btnDistancias.Text = "Distancias";
            this.btnDistancias.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnDistancias.UseVisualStyleBackColor = false;
            this.btnDistancias.Click += new System.EventHandler(this.btnDistancias_Click);
            // 
            // btnIterrupciones
            // 
            this.btnIterrupciones.BackColor = System.Drawing.Color.DarkSalmon;
            this.btnIterrupciones.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnIterrupciones.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnIterrupciones.ForeColor = System.Drawing.Color.Black;
            this.btnIterrupciones.Image = ((System.Drawing.Image)(resources.GetObject("btnIterrupciones.Image")));
            this.btnIterrupciones.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnIterrupciones.Location = new System.Drawing.Point(21, 120);
            this.btnIterrupciones.Name = "btnIterrupciones";
            this.btnIterrupciones.Size = new System.Drawing.Size(145, 28);
            this.btnIterrupciones.TabIndex = 13;
            this.btnIterrupciones.Text = "Interrupciones";
            this.btnIterrupciones.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnIterrupciones.UseVisualStyleBackColor = false;
            this.btnIterrupciones.Click += new System.EventHandler(this.BtnIterrupciones_Click);
            // 
            // btnValoracion
            // 
            this.btnValoracion.BackColor = System.Drawing.Color.DarkSalmon;
            this.btnValoracion.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnValoracion.Font = new System.Drawing.Font("Verdana", 8F);
            this.btnValoracion.ForeColor = System.Drawing.Color.Black;
            this.btnValoracion.Image = ((System.Drawing.Image)(resources.GetObject("btnValoracion.Image")));
            this.btnValoracion.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnValoracion.Location = new System.Drawing.Point(182, 56);
            this.btnValoracion.Name = "btnValoracion";
            this.btnValoracion.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.btnValoracion.Size = new System.Drawing.Size(145, 28);
            this.btnValoracion.TabIndex = 11;
            this.btnValoracion.Text = "Valoración";
            this.btnValoracion.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnValoracion.UseVisualStyleBackColor = false;
            this.btnValoracion.Click += new System.EventHandler(this.btnValoracion_Click);
            // 
            // btnTolGrupo
            // 
            this.btnTolGrupo.BackColor = System.Drawing.Color.LightGoldenrodYellow;
            this.btnTolGrupo.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnTolGrupo.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnTolGrupo.ForeColor = System.Drawing.Color.Maroon;
            this.btnTolGrupo.Image = ((System.Drawing.Image)(resources.GetObject("btnTolGrupo.Image")));
            this.btnTolGrupo.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnTolGrupo.Location = new System.Drawing.Point(21, 264);
            this.btnTolGrupo.Name = "btnTolGrupo";
            this.btnTolGrupo.Size = new System.Drawing.Size(145, 34);
            this.btnTolGrupo.TabIndex = 10;
            this.btnTolGrupo.Text = "Tolerancias";
            this.btnTolGrupo.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnTolGrupo.UseVisualStyleBackColor = false;
            this.btnTolGrupo.Click += new System.EventHandler(this.BtnTolGrupoClick);
            // 
            // btnPesosNum
            // 
            this.btnPesosNum.BackColor = System.Drawing.Color.DarkSalmon;
            this.btnPesosNum.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnPesosNum.Font = new System.Drawing.Font("Verdana", 8F);
            this.btnPesosNum.ForeColor = System.Drawing.Color.Black;
            this.btnPesosNum.Image = ((System.Drawing.Image)(resources.GetObject("btnPesosNum.Image")));
            this.btnPesosNum.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnPesosNum.Location = new System.Drawing.Point(182, 24);
            this.btnPesosNum.Name = "btnPesosNum";
            this.btnPesosNum.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.btnPesosNum.Size = new System.Drawing.Size(145, 28);
            this.btnPesosNum.TabIndex = 8;
            this.btnPesosNum.Text = "Pesos Numéricos";
            this.btnPesosNum.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnPesosNum.UseVisualStyleBackColor = false;
            this.btnPesosNum.Click += new System.EventHandler(this.BtnPesosNumClick);
            // 
            // btnCP
            // 
            this.btnCP.BackColor = System.Drawing.Color.DarkSalmon;
            this.btnCP.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnCP.Font = new System.Drawing.Font("Verdana", 8F);
            this.btnCP.ForeColor = System.Drawing.Color.Black;
            this.btnCP.Image = ((System.Drawing.Image)(resources.GetObject("btnCP.Image")));
            this.btnCP.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCP.Location = new System.Drawing.Point(182, 88);
            this.btnCP.Name = "btnCP";
            this.btnCP.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.btnCP.Size = new System.Drawing.Size(145, 28);
            this.btnCP.TabIndex = 7;
            this.btnCP.Text = "Columnas Probables";
            this.btnCP.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnCP.UseVisualStyleBackColor = false;
            this.btnCP.Click += new System.EventHandler(this.BtnCPClick);
            // 
            // btnDibujos
            // 
            this.btnDibujos.BackColor = System.Drawing.Color.DarkSalmon;
            this.btnDibujos.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnDibujos.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDibujos.ForeColor = System.Drawing.Color.Black;
            this.btnDibujos.Image = ((System.Drawing.Image)(resources.GetObject("btnDibujos.Image")));
            this.btnDibujos.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnDibujos.Location = new System.Drawing.Point(21, 88);
            this.btnDibujos.Name = "btnDibujos";
            this.btnDibujos.Size = new System.Drawing.Size(145, 28);
            this.btnDibujos.TabIndex = 4;
            this.btnDibujos.Text = "Dibujos";
            this.btnDibujos.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnDibujos.UseVisualStyleBackColor = false;
            this.btnDibujos.Click += new System.EventHandler(this.BtnDibujosClick);
            // 
            // btnSignosSeguidos
            // 
            this.btnSignosSeguidos.BackColor = System.Drawing.Color.DarkSalmon;
            this.btnSignosSeguidos.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnSignosSeguidos.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSignosSeguidos.ForeColor = System.Drawing.Color.Black;
            this.btnSignosSeguidos.Image = ((System.Drawing.Image)(resources.GetObject("btnSignosSeguidos.Image")));
            this.btnSignosSeguidos.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSignosSeguidos.Location = new System.Drawing.Point(21, 56);
            this.btnSignosSeguidos.Name = "btnSignosSeguidos";
            this.btnSignosSeguidos.Size = new System.Drawing.Size(145, 28);
            this.btnSignosSeguidos.TabIndex = 1;
            this.btnSignosSeguidos.Text = "Signos Seguidos";
            this.btnSignosSeguidos.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnSignosSeguidos.UseVisualStyleBackColor = false;
            this.btnSignosSeguidos.Click += new System.EventHandler(this.BtnSignosSeguidosClick);
            // 
            // btnNoVariantes
            // 
            this.btnNoVariantes.BackColor = System.Drawing.Color.DarkSalmon;
            this.btnNoVariantes.CausesValidation = false;
            this.btnNoVariantes.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnNoVariantes.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnNoVariantes.ForeColor = System.Drawing.Color.Black;
            this.btnNoVariantes.Image = ((System.Drawing.Image)(resources.GetObject("btnNoVariantes.Image")));
            this.btnNoVariantes.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnNoVariantes.Location = new System.Drawing.Point(21, 24);
            this.btnNoVariantes.Name = "btnNoVariantes";
            this.btnNoVariantes.Size = new System.Drawing.Size(145, 28);
            this.btnNoVariantes.TabIndex = 0;
            this.btnNoVariantes.Text = "Variantes, X y 2";
            this.btnNoVariantes.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnNoVariantes.UseVisualStyleBackColor = false;
            this.btnNoVariantes.Click += new System.EventHandler(this.BtnNoVariantesClick);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btnBorrarGrupo);
            this.groupBox2.Controls.Add(this.btnInsertarGrupo);
            this.groupBox2.Controls.Add(this.btnPegarGrupo);
            this.groupBox2.Controls.Add(this.btnCopiarGrupo);
            this.groupBox2.Controls.Add(this.btnGuardarGrupo);
            this.groupBox2.Controls.Add(this.btnAbrirGrupo);
            this.groupBox2.Controls.Add(this.btnGrupoPrevM);
            this.groupBox2.Controls.Add(this.btnGrupoInicio);
            this.groupBox2.Controls.Add(this.btnGrupoFin);
            this.groupBox2.Controls.Add(this.btnGrupoSiguienteM);
            this.groupBox2.Controls.Add(this.btnControlGrupos);
            this.groupBox2.Controls.Add(this.btnGrupoSiguiente);
            this.groupBox2.Controls.Add(this.btnGrupoPrev);
            this.groupBox2.Controls.Add(this.pronosticos);
            this.groupBox2.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox2.ForeColor = System.Drawing.Color.Maroon;
            this.groupBox2.Location = new System.Drawing.Point(41, 17);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(370, 492);
            this.groupBox2.TabIndex = 5;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Pronósticos";
            // 
            // btnBorrarGrupo
            // 
            this.btnBorrarGrupo.BackColor = System.Drawing.Color.LightSalmon;
            this.btnBorrarGrupo.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnBorrarGrupo.ForeColor = System.Drawing.Color.Black;
            this.btnBorrarGrupo.Image = ((System.Drawing.Image)(resources.GetObject("btnBorrarGrupo.Image")));
            this.btnBorrarGrupo.Location = new System.Drawing.Point(236, 453);
            this.btnBorrarGrupo.Name = "btnBorrarGrupo";
            this.btnBorrarGrupo.Size = new System.Drawing.Size(24, 24);
            this.btnBorrarGrupo.TabIndex = 29;
            this.btnBorrarGrupo.Tag = "+";
            this.btnBorrarGrupo.UseVisualStyleBackColor = false;
            this.btnBorrarGrupo.Visible = false;
            this.btnBorrarGrupo.Click += new System.EventHandler(this.mEliminarGrupos);
            // 
            // btnInsertarGrupo
            // 
            this.btnInsertarGrupo.BackColor = System.Drawing.Color.LightSalmon;
            this.btnInsertarGrupo.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnInsertarGrupo.ForeColor = System.Drawing.Color.Black;
            this.btnInsertarGrupo.Image = ((System.Drawing.Image)(resources.GetObject("btnInsertarGrupo.Image")));
            this.btnInsertarGrupo.Location = new System.Drawing.Point(211, 453);
            this.btnInsertarGrupo.Name = "btnInsertarGrupo";
            this.btnInsertarGrupo.Size = new System.Drawing.Size(24, 24);
            this.btnInsertarGrupo.TabIndex = 28;
            this.btnInsertarGrupo.Tag = "+";
            this.btnInsertarGrupo.UseVisualStyleBackColor = false;
            this.btnInsertarGrupo.Visible = false;
            this.btnInsertarGrupo.Click += new System.EventHandler(this.mInsertarGrupos);
            // 
            // btnPegarGrupo
            // 
            this.btnPegarGrupo.BackColor = System.Drawing.Color.LightSalmon;
            this.btnPegarGrupo.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnPegarGrupo.ForeColor = System.Drawing.Color.Black;
            this.btnPegarGrupo.Image = ((System.Drawing.Image)(resources.GetObject("btnPegarGrupo.Image")));
            this.btnPegarGrupo.Location = new System.Drawing.Point(186, 453);
            this.btnPegarGrupo.Name = "btnPegarGrupo";
            this.btnPegarGrupo.Size = new System.Drawing.Size(24, 24);
            this.btnPegarGrupo.TabIndex = 27;
            this.btnPegarGrupo.Tag = "+";
            this.btnPegarGrupo.UseVisualStyleBackColor = false;
            this.btnPegarGrupo.Visible = false;
            this.btnPegarGrupo.Click += new System.EventHandler(this.mPegarGrupos);
            // 
            // btnCopiarGrupo
            // 
            this.btnCopiarGrupo.BackColor = System.Drawing.Color.LightSalmon;
            this.btnCopiarGrupo.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnCopiarGrupo.ForeColor = System.Drawing.Color.Black;
            this.btnCopiarGrupo.Image = ((System.Drawing.Image)(resources.GetObject("btnCopiarGrupo.Image")));
            this.btnCopiarGrupo.Location = new System.Drawing.Point(161, 453);
            this.btnCopiarGrupo.Name = "btnCopiarGrupo";
            this.btnCopiarGrupo.Size = new System.Drawing.Size(24, 24);
            this.btnCopiarGrupo.TabIndex = 26;
            this.btnCopiarGrupo.Tag = "+";
            this.btnCopiarGrupo.UseVisualStyleBackColor = false;
            this.btnCopiarGrupo.Visible = false;
            this.btnCopiarGrupo.Click += new System.EventHandler(this.mCopiarGrupos);
            // 
            // btnGuardarGrupo
            // 
            this.btnGuardarGrupo.BackColor = System.Drawing.Color.LightSalmon;
            this.btnGuardarGrupo.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnGuardarGrupo.ForeColor = System.Drawing.Color.Black;
            this.btnGuardarGrupo.Image = ((System.Drawing.Image)(resources.GetObject("btnGuardarGrupo.Image")));
            this.btnGuardarGrupo.Location = new System.Drawing.Point(136, 453);
            this.btnGuardarGrupo.Name = "btnGuardarGrupo";
            this.btnGuardarGrupo.Size = new System.Drawing.Size(24, 24);
            this.btnGuardarGrupo.TabIndex = 25;
            this.btnGuardarGrupo.Tag = "+";
            this.btnGuardarGrupo.UseVisualStyleBackColor = false;
            this.btnGuardarGrupo.Visible = false;
            this.btnGuardarGrupo.Click += new System.EventHandler(this.mGuardarGrupos);
            // 
            // btnAbrirGrupo
            // 
            this.btnAbrirGrupo.BackColor = System.Drawing.Color.LightSalmon;
            this.btnAbrirGrupo.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnAbrirGrupo.ForeColor = System.Drawing.Color.Black;
            this.btnAbrirGrupo.Image = ((System.Drawing.Image)(resources.GetObject("btnAbrirGrupo.Image")));
            this.btnAbrirGrupo.Location = new System.Drawing.Point(111, 453);
            this.btnAbrirGrupo.Name = "btnAbrirGrupo";
            this.btnAbrirGrupo.Size = new System.Drawing.Size(24, 24);
            this.btnAbrirGrupo.TabIndex = 24;
            this.btnAbrirGrupo.Tag = "+";
            this.btnAbrirGrupo.UseVisualStyleBackColor = false;
            this.btnAbrirGrupo.Visible = false;
            this.btnAbrirGrupo.Click += new System.EventHandler(this.mAbrirGrupos);
            // 
            // btnGrupoPrevM
            // 
            this.btnGrupoPrevM.BackColor = System.Drawing.Color.Silver;
            this.btnGrupoPrevM.Enabled = false;
            this.btnGrupoPrevM.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnGrupoPrevM.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.btnGrupoPrevM.ForeColor = System.Drawing.Color.Black;
            this.btnGrupoPrevM.Image = ((System.Drawing.Image)(resources.GetObject("btnGrupoPrevM.Image")));
            this.btnGrupoPrevM.Location = new System.Drawing.Point(42, 418);
            this.btnGrupoPrevM.Name = "btnGrupoPrevM";
            this.btnGrupoPrevM.Size = new System.Drawing.Size(24, 24);
            this.btnGrupoPrevM.TabIndex = 16;
            this.btnGrupoPrevM.UseVisualStyleBackColor = false;
            this.btnGrupoPrevM.Click += new System.EventHandler(this.btnGrupoPrevM_Click);
            this.btnGrupoPrevM.EnabledChanged += new System.EventHandler(this.HabilitarBoton);
            // 
            // btnGrupoInicio
            // 
            this.btnGrupoInicio.BackColor = System.Drawing.Color.Silver;
            this.btnGrupoInicio.Enabled = false;
            this.btnGrupoInicio.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnGrupoInicio.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.btnGrupoInicio.ForeColor = System.Drawing.Color.Black;
            this.btnGrupoInicio.Image = ((System.Drawing.Image)(resources.GetObject("btnGrupoInicio.Image")));
            this.btnGrupoInicio.Location = new System.Drawing.Point(17, 418);
            this.btnGrupoInicio.Name = "btnGrupoInicio";
            this.btnGrupoInicio.Size = new System.Drawing.Size(24, 24);
            this.btnGrupoInicio.TabIndex = 15;
            this.btnGrupoInicio.UseVisualStyleBackColor = false;
            this.btnGrupoInicio.Click += new System.EventHandler(this.btnGrupoInicio_Click);
            this.btnGrupoInicio.EnabledChanged += new System.EventHandler(this.HabilitarBoton);
            // 
            // btnGrupoFin
            // 
            this.btnGrupoFin.BackColor = System.Drawing.Color.Silver;
            this.btnGrupoFin.Enabled = false;
            this.btnGrupoFin.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnGrupoFin.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.btnGrupoFin.ForeColor = System.Drawing.Color.Black;
            this.btnGrupoFin.Image = ((System.Drawing.Image)(resources.GetObject("btnGrupoFin.Image")));
            this.btnGrupoFin.Location = new System.Drawing.Point(149, 418);
            this.btnGrupoFin.Name = "btnGrupoFin";
            this.btnGrupoFin.Size = new System.Drawing.Size(24, 24);
            this.btnGrupoFin.TabIndex = 14;
            this.btnGrupoFin.UseVisualStyleBackColor = false;
            this.btnGrupoFin.Click += new System.EventHandler(this.btnGrupoFin_Click);
            this.btnGrupoFin.EnabledChanged += new System.EventHandler(this.HabilitarBoton);
            // 
            // btnGrupoSiguienteM
            // 
            this.btnGrupoSiguienteM.BackColor = System.Drawing.Color.Silver;
            this.btnGrupoSiguienteM.Enabled = false;
            this.btnGrupoSiguienteM.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnGrupoSiguienteM.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.btnGrupoSiguienteM.ForeColor = System.Drawing.Color.Black;
            this.btnGrupoSiguienteM.Image = ((System.Drawing.Image)(resources.GetObject("btnGrupoSiguienteM.Image")));
            this.btnGrupoSiguienteM.Location = new System.Drawing.Point(124, 418);
            this.btnGrupoSiguienteM.Name = "btnGrupoSiguienteM";
            this.btnGrupoSiguienteM.Size = new System.Drawing.Size(24, 24);
            this.btnGrupoSiguienteM.TabIndex = 13;
            this.btnGrupoSiguienteM.UseVisualStyleBackColor = false;
            this.btnGrupoSiguienteM.Click += new System.EventHandler(this.btnGrupoSiguienteM_Click);
            this.btnGrupoSiguienteM.EnabledChanged += new System.EventHandler(this.HabilitarBoton);
            // 
            // btnControlGrupos
            // 
            this.btnControlGrupos.BackColor = System.Drawing.Color.DarkSalmon;
            this.btnControlGrupos.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnControlGrupos.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnControlGrupos.ForeColor = System.Drawing.Color.Black;
            this.btnControlGrupos.Image = ((System.Drawing.Image)(resources.GetObject("btnControlGrupos.Image")));
            this.btnControlGrupos.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnControlGrupos.Location = new System.Drawing.Point(211, 414);
            this.btnControlGrupos.Name = "btnControlGrupos";
            this.btnControlGrupos.Size = new System.Drawing.Size(142, 32);
            this.btnControlGrupos.TabIndex = 3;
            this.btnControlGrupos.Text = "Control de Grupos";
            this.btnControlGrupos.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnControlGrupos.UseVisualStyleBackColor = false;
            this.btnControlGrupos.Click += new System.EventHandler(this.BtnControlGruposClick);
            // 
            // btnGrupoSiguiente
            // 
            this.btnGrupoSiguiente.BackColor = System.Drawing.Color.LightSalmon;
            this.btnGrupoSiguiente.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnGrupoSiguiente.ForeColor = System.Drawing.Color.Black;
            this.btnGrupoSiguiente.Image = ((System.Drawing.Image)(resources.GetObject("btnGrupoSiguiente.Image")));
            this.btnGrupoSiguiente.Location = new System.Drawing.Point(99, 418);
            this.btnGrupoSiguiente.Name = "btnGrupoSiguiente";
            this.btnGrupoSiguiente.Size = new System.Drawing.Size(24, 24);
            this.btnGrupoSiguiente.TabIndex = 2;
            this.btnGrupoSiguiente.UseVisualStyleBackColor = false;
            this.btnGrupoSiguiente.Click += new System.EventHandler(this.BtnGrupoSiguienteClick);
            // 
            // btnGrupoPrev
            // 
            this.btnGrupoPrev.BackColor = System.Drawing.Color.Silver;
            this.btnGrupoPrev.Enabled = false;
            this.btnGrupoPrev.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnGrupoPrev.ForeColor = System.Drawing.Color.Black;
            this.btnGrupoPrev.Image = ((System.Drawing.Image)(resources.GetObject("btnGrupoPrev.Image")));
            this.btnGrupoPrev.Location = new System.Drawing.Point(67, 418);
            this.btnGrupoPrev.Name = "btnGrupoPrev";
            this.btnGrupoPrev.Size = new System.Drawing.Size(24, 24);
            this.btnGrupoPrev.TabIndex = 1;
            this.btnGrupoPrev.UseVisualStyleBackColor = false;
            this.btnGrupoPrev.Click += new System.EventHandler(this.BtnGrupoPrevClick);
            this.btnGrupoPrev.EnabledChanged += new System.EventHandler(this.HabilitarBoton);
            // 
            // gbFiltroGeneral
            // 
            this.gbFiltroGeneral.Controls.Add(this.lblMenosColumnas);
            this.gbFiltroGeneral.Controls.Add(this.txtCompletarCon);
            this.gbFiltroGeneral.Controls.Add(this.imgCerrar);
            this.gbFiltroGeneral.Controls.Add(this.imgAbrir);
            this.gbFiltroGeneral.Controls.Add(this.chkFiltroCols);
            this.gbFiltroGeneral.Controls.Add(this.lblNombreFiltro);
            this.gbFiltroGeneral.Controls.Add(this.btnAddFiltroCols);
            this.gbFiltroGeneral.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gbFiltroGeneral.ForeColor = System.Drawing.Color.Maroon;
            this.gbFiltroGeneral.Location = new System.Drawing.Point(417, 338);
            this.gbFiltroGeneral.Name = "gbFiltroGeneral";
            this.gbFiltroGeneral.Size = new System.Drawing.Size(343, 83);
            this.gbFiltroGeneral.TabIndex = 6;
            this.gbFiltroGeneral.TabStop = false;
            this.gbFiltroGeneral.Text = "Filtro";
            // 
            // lblMenosColumnas
            // 
            this.lblMenosColumnas.Font = new System.Drawing.Font("Verdana", 7F);
            this.lblMenosColumnas.ForeColor = System.Drawing.Color.Black;
            this.lblMenosColumnas.Location = new System.Drawing.Point(6, 49);
            this.lblMenosColumnas.Name = "lblMenosColumnas";
            this.lblMenosColumnas.Size = new System.Drawing.Size(253, 31);
            this.lblMenosColumnas.TabIndex = 9;
            this.lblMenosColumnas.Text = "Las columnas del filtro tienen menos signos. Completar con";
            this.lblMenosColumnas.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lblMenosColumnas.Visible = false;
            // 
            // txtCompletarCon
            // 
            this.txtCompletarCon.AcceptsReturn = true;
            this.txtCompletarCon.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtCompletarCon.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtCompletarCon.ForeColor = System.Drawing.Color.Black;
            this.txtCompletarCon.Location = new System.Drawing.Point(265, 54);
            this.txtCompletarCon.Name = "txtCompletarCon";
            this.txtCompletarCon.Size = new System.Drawing.Size(20, 21);
            this.txtCompletarCon.TabIndex = 8;
            this.txtCompletarCon.Visible = false;
            // 
            // imgCerrar
            // 
            this.imgCerrar.Image = ((System.Drawing.Image)(resources.GetObject("imgCerrar.Image")));
            this.imgCerrar.Location = new System.Drawing.Point(259, 8);
            this.imgCerrar.Name = "imgCerrar";
            this.imgCerrar.Size = new System.Drawing.Size(16, 16);
            this.imgCerrar.TabIndex = 5;
            this.imgCerrar.TabStop = false;
            this.imgCerrar.Visible = false;
            // 
            // imgAbrir
            // 
            this.imgAbrir.Image = ((System.Drawing.Image)(resources.GetObject("imgAbrir.Image")));
            this.imgAbrir.Location = new System.Drawing.Point(237, 8);
            this.imgAbrir.Name = "imgAbrir";
            this.imgAbrir.Size = new System.Drawing.Size(16, 16);
            this.imgAbrir.TabIndex = 4;
            this.imgAbrir.TabStop = false;
            this.imgAbrir.Visible = false;
            // 
            // btnAddFiltroCols
            // 
            this.btnAddFiltroCols.BackColor = System.Drawing.Color.LightSalmon;
            this.btnAddFiltroCols.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnAddFiltroCols.ForeColor = System.Drawing.Color.Black;
            this.btnAddFiltroCols.Image = ((System.Drawing.Image)(resources.GetObject("btnAddFiltroCols.Image")));
            this.btnAddFiltroCols.Location = new System.Drawing.Point(40, 24);
            this.btnAddFiltroCols.Name = "btnAddFiltroCols";
            this.btnAddFiltroCols.Size = new System.Drawing.Size(24, 20);
            this.btnAddFiltroCols.TabIndex = 0;
            this.btnAddFiltroCols.Tag = "+";
            this.btnAddFiltroCols.UseVisualStyleBackColor = false;
            this.btnAddFiltroCols.Click += new System.EventHandler(this.BtnAddFiltroColsClick);
            // 
            // ilUtilidades
            // 
            this.ilUtilidades.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            this.ilUtilidades.ImageSize = new System.Drawing.Size(26, 26);
            this.ilUtilidades.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // ilArchivo
            // 
            this.ilArchivo.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            this.ilArchivo.ImageSize = new System.Drawing.Size(26, 26);
            this.ilArchivo.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // ilFree
            // 
            this.ilFree.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            this.ilFree.ImageSize = new System.Drawing.Size(26, 26);
            this.ilFree.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // ilCombinacion
            // 
            this.ilCombinacion.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            this.ilCombinacion.ImageSize = new System.Drawing.Size(26, 26);
            this.ilCombinacion.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // ilFiltros
            // 
            this.ilFiltros.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            this.ilFiltros.ImageSize = new System.Drawing.Size(26, 26);
            this.ilFiltros.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // ilOperaciones
            // 
            this.ilOperaciones.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            this.ilOperaciones.ImageSize = new System.Drawing.Size(26, 26);
            this.ilOperaciones.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // ilGrupos
            // 
            this.ilGrupos.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            this.ilGrupos.ImageSize = new System.Drawing.Size(24, 24);
            this.ilGrupos.TransparentColor = System.Drawing.Color.Bisque;
            // 
            // toolStripContainer1
            // 
            this.toolStripContainer1.BottomToolStripPanelVisible = false;
            // 
            // toolStripContainer1.ContentPanel
            // 
            this.toolStripContainer1.ContentPanel.AutoScroll = true;
            this.toolStripContainer1.ContentPanel.Controls.Add(this.pctNotificaciones);
            this.toolStripContainer1.ContentPanel.Controls.Add(this.pBQuinielista);
            this.toolStripContainer1.ContentPanel.Controls.Add(this.gbFiltroParcial);
            this.toolStripContainer1.ContentPanel.Controls.Add(this.gbFiltroGeneral);
            this.toolStripContainer1.ContentPanel.Controls.Add(this.groupBox2);
            this.toolStripContainer1.ContentPanel.Controls.Add(this.groupBox);
            this.toolStripContainer1.ContentPanel.Margin = new System.Windows.Forms.Padding(0);
            this.toolStripContainer1.ContentPanel.Size = new System.Drawing.Size(779, 553);
            this.toolStripContainer1.Dock = System.Windows.Forms.DockStyle.Top;
            // 
            // toolStripContainer1.LeftToolStripPanel
            // 
            this.toolStripContainer1.LeftToolStripPanel.BackColor = System.Drawing.Color.Bisque;
            this.toolStripContainer1.LeftToolStripPanel.Enabled = false;
            this.toolStripContainer1.LeftToolStripPanelVisible = false;
            this.toolStripContainer1.Location = new System.Drawing.Point(0, 0);
            this.toolStripContainer1.MinimumSize = new System.Drawing.Size(0, 30);
            this.toolStripContainer1.Name = "toolStripContainer1";
            this.toolStripContainer1.RightToolStripPanelVisible = false;
            this.toolStripContainer1.Size = new System.Drawing.Size(779, 603);
            this.toolStripContainer1.TabIndex = 19;
            this.toolStripContainer1.Text = "toolStripContainer1";
            // 
            // toolStripContainer1.TopToolStripPanel
            // 
            this.toolStripContainer1.TopToolStripPanel.BackColor = System.Drawing.Color.Bisque;
            this.toolStripContainer1.TopToolStripPanel.MinimumSize = new System.Drawing.Size(20, 50);
            // 
            // pBQuinielista
            // 
            this.pBQuinielista.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pBQuinielista.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pBQuinielista.Image = ((System.Drawing.Image)(resources.GetObject("pBQuinielista.Image")));
            this.pBQuinielista.Location = new System.Drawing.Point(558, 515);
            this.pBQuinielista.Name = "pBQuinielista";
            this.pBQuinielista.Size = new System.Drawing.Size(190, 28);
            this.pBQuinielista.TabIndex = 8;
            this.pBQuinielista.TabStop = false;
            this.pBQuinielista.Click += new System.EventHandler(this.pBQuinielista_Click);
            // 
            // gbFiltroParcial
            // 
            this.gbFiltroParcial.Controls.Add(this.pictureBox1);
            this.gbFiltroParcial.Controls.Add(this.pictureBox2);
            this.gbFiltroParcial.Controls.Add(this.chkFiltroColsParcial);
            this.gbFiltroParcial.Controls.Add(this.lblNombreFiltroParcial);
            this.gbFiltroParcial.Controls.Add(this.btnAbreFiltroParcial);
            this.gbFiltroParcial.Enabled = false;
            this.gbFiltroParcial.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gbFiltroParcial.ForeColor = System.Drawing.Color.Maroon;
            this.gbFiltroParcial.Location = new System.Drawing.Point(417, 427);
            this.gbFiltroParcial.Name = "gbFiltroParcial";
            this.gbFiltroParcial.Size = new System.Drawing.Size(343, 82);
            this.gbFiltroParcial.TabIndex = 7;
            this.gbFiltroParcial.TabStop = false;
            this.gbFiltroParcial.Text = "Filtro Parcial del Grupo";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(259, 8);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(16, 16);
            this.pictureBox1.TabIndex = 5;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Visible = false;
            // 
            // pictureBox2
            // 
            this.pictureBox2.Location = new System.Drawing.Point(237, 8);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(16, 16);
            this.pictureBox2.TabIndex = 4;
            this.pictureBox2.TabStop = false;
            this.pictureBox2.Visible = false;
            // 
            // lblNombreFiltroParcial
            // 
            this.lblNombreFiltroParcial.Font = new System.Drawing.Font("Verdana", 7F);
            this.lblNombreFiltroParcial.ForeColor = System.Drawing.Color.Black;
            this.lblNombreFiltroParcial.Location = new System.Drawing.Point(72, 25);
            this.lblNombreFiltroParcial.Name = "lblNombreFiltroParcial";
            this.lblNombreFiltroParcial.Size = new System.Drawing.Size(230, 16);
            this.lblNombreFiltroParcial.TabIndex = 1;
            this.lblNombreFiltroParcial.Text = "(Selecciona)";
            this.lblNombreFiltroParcial.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnAbreFiltroParcial
            // 
            this.btnAbreFiltroParcial.BackColor = System.Drawing.Color.LightSalmon;
            this.btnAbreFiltroParcial.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnAbreFiltroParcial.ForeColor = System.Drawing.Color.Black;
            this.btnAbreFiltroParcial.Image = ((System.Drawing.Image)(resources.GetObject("btnAbreFiltroParcial.Image")));
            this.btnAbreFiltroParcial.Location = new System.Drawing.Point(40, 24);
            this.btnAbreFiltroParcial.Name = "btnAbreFiltroParcial";
            this.btnAbreFiltroParcial.Size = new System.Drawing.Size(24, 20);
            this.btnAbreFiltroParcial.TabIndex = 0;
            this.btnAbreFiltroParcial.Tag = "+";
            this.btnAbreFiltroParcial.UseVisualStyleBackColor = false;
            this.btnAbreFiltroParcial.Click += new System.EventHandler(this.btnAbreFiltroParcial_Click);
            // 
            // mainMenu
            // 
            this.mainMenu.Dock = System.Windows.Forms.DockStyle.None;
            this.mainMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuFree1X2,
            this.menuArchivo,
            this.menuVer,
            this.menuCombinacion,
            this.menuGrupo,
            this.menuFiltros,
            this.menuOperaciones,
            this.menuUtilidades});
            this.mainMenu.Location = new System.Drawing.Point(0, 0);
            this.mainMenu.Name = "mainMenu";
            this.mainMenu.Padding = new System.Windows.Forms.Padding(0);
            this.mainMenu.Size = new System.Drawing.Size(771, 24);
            this.mainMenu.TabIndex = 0;
            this.mainMenu.Text = "mainMenu";
            // 
            // menuFree1X2
            // 
            this.menuFree1X2.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItem42,
            this.menuItem43,
            this.configurarAnálisisToolStripMenuItem,
            this.menuItem44,
            this.menuItem53,
            this.comprobarActualizacionesToolStripMenuItem});
            this.menuFree1X2.Image = ((System.Drawing.Image)(resources.GetObject("menuFree1X2.Image")));
            this.menuFree1X2.Name = "menuFree1X2";
            this.menuFree1X2.Size = new System.Drawing.Size(75, 24);
            this.menuFree1X2.Text = "Free1X2";
            // 
            // menuItem42
            // 
            this.menuItem42.Image = ((System.Drawing.Image)(resources.GetObject("menuItem42.Image")));
            this.menuItem42.Name = "menuItem42";
            this.menuItem42.Size = new System.Drawing.Size(214, 22);
            this.menuItem42.Text = "Salir";
            this.menuItem42.Click += new System.EventHandler(this.MSalir);
            // 
            // menuItem43
            // 
            this.menuItem43.Image = ((System.Drawing.Image)(resources.GetObject("menuItem43.Image")));
            this.menuItem43.Name = "menuItem43";
            this.menuItem43.Size = new System.Drawing.Size(214, 22);
            this.menuItem43.Text = "Configuración";
            this.menuItem43.Click += new System.EventHandler(this.MConfiguracion);
            // 
            // configurarAnálisisToolStripMenuItem
            // 
            this.configurarAnálisisToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("configurarAnálisisToolStripMenuItem.Image")));
            this.configurarAnálisisToolStripMenuItem.Name = "configurarAnálisisToolStripMenuItem";
            this.configurarAnálisisToolStripMenuItem.Size = new System.Drawing.Size(214, 22);
            this.configurarAnálisisToolStripMenuItem.Text = "Configurar Análisis";
            this.configurarAnálisisToolStripMenuItem.Click += new System.EventHandler(this.configurarAnálisisToolStripMenuItem_Click);
            // 
            // menuItem44
            // 
            this.menuItem44.Image = ((System.Drawing.Image)(resources.GetObject("menuItem44.Image")));
            this.menuItem44.Name = "menuItem44";
            this.menuItem44.Size = new System.Drawing.Size(214, 22);
            this.menuItem44.Text = "Ayuda";
            this.menuItem44.Click += new System.EventHandler(this.MAyuda);
            // 
            // menuItem53
            // 
            this.menuItem53.Image = ((System.Drawing.Image)(resources.GetObject("menuItem53.Image")));
            this.menuItem53.Name = "menuItem53";
            this.menuItem53.Size = new System.Drawing.Size(214, 22);
            this.menuItem53.Text = "Acerca de";
            this.menuItem53.Click += new System.EventHandler(this.mAcercaDe);
            // 
            // comprobarActualizacionesToolStripMenuItem
            // 
            this.comprobarActualizacionesToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("comprobarActualizacionesToolStripMenuItem.Image")));
            this.comprobarActualizacionesToolStripMenuItem.Name = "comprobarActualizacionesToolStripMenuItem";
            this.comprobarActualizacionesToolStripMenuItem.Size = new System.Drawing.Size(214, 22);
            this.comprobarActualizacionesToolStripMenuItem.Text = "Comprobar Actualizaciones";
            this.comprobarActualizacionesToolStripMenuItem.Click += new System.EventHandler(this.comprobarActualizacionesToolStripMenuItem_Click);
            // 
            // menuArchivo
            // 
            this.menuArchivo.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItem8,
            this.menuItem9,
            this.obtenerBoletosOnlineToolStripMenuItem,
            this.menuItem3,
            this.mAbrirComb,
            this.menuItem2,
            this.menuItem7,
            this.menuItem52,
            this.borrarInformesDeErrorToolStripMenuItem,
            this.gestiónDeEquiposToolStripMenuItem});
            this.menuArchivo.Image = ((System.Drawing.Image)(resources.GetObject("menuArchivo.Image")));
            this.menuArchivo.Name = "menuArchivo";
            this.menuArchivo.Size = new System.Drawing.Size(71, 24);
            this.menuArchivo.Text = "Archivo";
            // 
            // menuItem8
            // 
            this.menuItem8.Image = ((System.Drawing.Image)(resources.GetObject("menuItem8.Image")));
            this.menuItem8.Name = "menuItem8";
            this.menuItem8.Size = new System.Drawing.Size(247, 22);
            this.menuItem8.Text = "Abrir Partidos Boleto";
            this.menuItem8.Click += new System.EventHandler(this.MAbreBoleto);
            // 
            // menuItem9
            // 
            this.menuItem9.Image = ((System.Drawing.Image)(resources.GetObject("menuItem9.Image")));
            this.menuItem9.Name = "menuItem9";
            this.menuItem9.Size = new System.Drawing.Size(247, 22);
            this.menuItem9.Text = "Guardar Partidos Boleto";
            this.menuItem9.Click += new System.EventHandler(this.MGuardarPartidosClick);
            // 
            // obtenerBoletosOnlineToolStripMenuItem
            // 
            this.obtenerBoletosOnlineToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("obtenerBoletosOnlineToolStripMenuItem.Image")));
            this.obtenerBoletosOnlineToolStripMenuItem.Name = "obtenerBoletosOnlineToolStripMenuItem";
            this.obtenerBoletosOnlineToolStripMenuItem.Size = new System.Drawing.Size(247, 22);
            this.obtenerBoletosOnlineToolStripMenuItem.Text = "Obtener Boletos Online";
            this.obtenerBoletosOnlineToolStripMenuItem.Click += new System.EventHandler(this.obtenerBoletosOnlineToolStripMenuItem_Click);
            // 
            // menuItem3
            // 
            this.menuItem3.Image = ((System.Drawing.Image)(resources.GetObject("menuItem3.Image")));
            this.menuItem3.Name = "menuItem3";
            this.menuItem3.Size = new System.Drawing.Size(247, 22);
            this.menuItem3.Text = "Nueva Combinación";
            this.menuItem3.Click += new System.EventHandler(this.MNuevaComb);
            // 
            // mAbrirComb
            // 
            this.mAbrirComb.Image = ((System.Drawing.Image)(resources.GetObject("mAbrirComb.Image")));
            this.mAbrirComb.Name = "mAbrirComb";
            this.mAbrirComb.Size = new System.Drawing.Size(247, 22);
            this.mAbrirComb.Text = "Abrir Combinación";
            this.mAbrirComb.Click += new System.EventHandler(this.MAbrirCombClick);
            // 
            // menuItem2
            // 
            this.menuItem2.Image = ((System.Drawing.Image)(resources.GetObject("menuItem2.Image")));
            this.menuItem2.Name = "menuItem2";
            this.menuItem2.Size = new System.Drawing.Size(247, 22);
            this.menuItem2.Text = "Guardar Combinación";
            this.menuItem2.Click += new System.EventHandler(this.MGuardarComb);
            // 
            // menuItem7
            // 
            this.menuItem7.Image = ((System.Drawing.Image)(resources.GetObject("menuItem7.Image")));
            this.menuItem7.Name = "menuItem7";
            this.menuItem7.Size = new System.Drawing.Size(247, 22);
            this.menuItem7.Text = "Guardar Combinación Como...";
            this.menuItem7.Click += new System.EventHandler(this.MGuardarCombComo);
            // 
            // menuItem52
            // 
            this.menuItem52.Image = ((System.Drawing.Image)(resources.GetObject("menuItem52.Image")));
            this.menuItem52.Name = "menuItem52";
            this.menuItem52.Size = new System.Drawing.Size(247, 22);
            this.menuItem52.Text = "Borrar Combinaciones Temporales";
            this.menuItem52.Click += new System.EventHandler(this.MBorrarCombsTemp);
            // 
            // borrarInformesDeErrorToolStripMenuItem
            // 
            this.borrarInformesDeErrorToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("borrarInformesDeErrorToolStripMenuItem.Image")));
            this.borrarInformesDeErrorToolStripMenuItem.Name = "borrarInformesDeErrorToolStripMenuItem";
            this.borrarInformesDeErrorToolStripMenuItem.Size = new System.Drawing.Size(247, 22);
            this.borrarInformesDeErrorToolStripMenuItem.Text = "Borrar Informes de Error";
            this.borrarInformesDeErrorToolStripMenuItem.Click += new System.EventHandler(this.borrarInformesDeErrorToolStripMenuItem_Click);
            // 
            // gestiónDeEquiposToolStripMenuItem
            // 
            this.gestiónDeEquiposToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("gestiónDeEquiposToolStripMenuItem.Image")));
            this.gestiónDeEquiposToolStripMenuItem.Name = "gestiónDeEquiposToolStripMenuItem";
            this.gestiónDeEquiposToolStripMenuItem.Size = new System.Drawing.Size(247, 22);
            this.gestiónDeEquiposToolStripMenuItem.Text = "Gestión de Equipos";
            this.gestiónDeEquiposToolStripMenuItem.Click += new System.EventHandler(this.gestiónDeEquiposToolStripMenuItem_Click);
            // 
            // menuVer
            // 
            this.menuVer.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.personalizarToolStripMenuItem,
            this.listadoDeCondicionesToolStripMenuItem});
            this.menuVer.Image = ((System.Drawing.Image)(resources.GetObject("menuVer.Image")));
            this.menuVer.Name = "menuVer";
            this.menuVer.Size = new System.Drawing.Size(51, 24);
            this.menuVer.Text = "Ver";
            // 
            // personalizarToolStripMenuItem
            // 
            this.personalizarToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.filtrosToolStripMenuItem,
            this.free1X2ToolStripMenuItem,
            this.operacionesToolStripMenuItem,
            this.utilidadesToolStripMenuItem,
            this.combinacionToolStripMenuItem,
            this.archivoToolStripMenuItem});
            this.personalizarToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("personalizarToolStripMenuItem.Image")));
            this.personalizarToolStripMenuItem.Name = "personalizarToolStripMenuItem";
            this.personalizarToolStripMenuItem.Size = new System.Drawing.Size(198, 22);
            this.personalizarToolStripMenuItem.Text = "Barras de Herramientas";
            // 
            // filtrosToolStripMenuItem
            // 
            this.filtrosToolStripMenuItem.CheckOnClick = true;
            this.filtrosToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("filtrosToolStripMenuItem.Image")));
            this.filtrosToolStripMenuItem.Name = "filtrosToolStripMenuItem";
            this.filtrosToolStripMenuItem.Size = new System.Drawing.Size(145, 22);
            this.filtrosToolStripMenuItem.Text = "Filtros";
            this.filtrosToolStripMenuItem.Click += new System.EventHandler(this.filtrosToolStripMenuItem_Click);
            // 
            // free1X2ToolStripMenuItem
            // 
            this.free1X2ToolStripMenuItem.CheckOnClick = true;
            this.free1X2ToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("free1X2ToolStripMenuItem.Image")));
            this.free1X2ToolStripMenuItem.Name = "free1X2ToolStripMenuItem";
            this.free1X2ToolStripMenuItem.Size = new System.Drawing.Size(145, 22);
            this.free1X2ToolStripMenuItem.Text = "Free1X2";
            this.free1X2ToolStripMenuItem.Click += new System.EventHandler(this.free1X2ToolStripMenuItem_Click);
            // 
            // operacionesToolStripMenuItem
            // 
            this.operacionesToolStripMenuItem.CheckOnClick = true;
            this.operacionesToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("operacionesToolStripMenuItem.Image")));
            this.operacionesToolStripMenuItem.Name = "operacionesToolStripMenuItem";
            this.operacionesToolStripMenuItem.Size = new System.Drawing.Size(145, 22);
            this.operacionesToolStripMenuItem.Text = "Operaciones";
            this.operacionesToolStripMenuItem.Click += new System.EventHandler(this.operacionesToolStripMenuItem_Click);
            // 
            // utilidadesToolStripMenuItem
            // 
            this.utilidadesToolStripMenuItem.CheckOnClick = true;
            this.utilidadesToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("utilidadesToolStripMenuItem.Image")));
            this.utilidadesToolStripMenuItem.Name = "utilidadesToolStripMenuItem";
            this.utilidadesToolStripMenuItem.Size = new System.Drawing.Size(145, 22);
            this.utilidadesToolStripMenuItem.Text = "Utilidades";
            this.utilidadesToolStripMenuItem.Click += new System.EventHandler(this.utilidadesToolStripMenuItem_Click);
            // 
            // combinacionToolStripMenuItem
            // 
            this.combinacionToolStripMenuItem.Checked = true;
            this.combinacionToolStripMenuItem.CheckOnClick = true;
            this.combinacionToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.combinacionToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("combinacionToolStripMenuItem.Image")));
            this.combinacionToolStripMenuItem.Name = "combinacionToolStripMenuItem";
            this.combinacionToolStripMenuItem.Size = new System.Drawing.Size(145, 22);
            this.combinacionToolStripMenuItem.Text = "Combinación";
            this.combinacionToolStripMenuItem.Click += new System.EventHandler(this.combinaciónToolStripMenuItem_Click);
            // 
            // archivoToolStripMenuItem
            // 
            this.archivoToolStripMenuItem.CheckOnClick = true;
            this.archivoToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("archivoToolStripMenuItem.Image")));
            this.archivoToolStripMenuItem.Name = "archivoToolStripMenuItem";
            this.archivoToolStripMenuItem.Size = new System.Drawing.Size(145, 22);
            this.archivoToolStripMenuItem.Text = "Archivo";
            this.archivoToolStripMenuItem.Click += new System.EventHandler(this.archivoToolStripMenuItem_Click);
            // 
            // listadoDeCondicionesToolStripMenuItem
            // 
            this.listadoDeCondicionesToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("listadoDeCondicionesToolStripMenuItem.Image")));
            this.listadoDeCondicionesToolStripMenuItem.Name = "listadoDeCondicionesToolStripMenuItem";
            this.listadoDeCondicionesToolStripMenuItem.Size = new System.Drawing.Size(198, 22);
            this.listadoDeCondicionesToolStripMenuItem.Text = "Listado de condiciones";
            this.listadoDeCondicionesToolStripMenuItem.Click += new System.EventHandler(this.listadoDeCondicionesToolStripMenuItem_Click);
            // 
            // menuCombinacion
            // 
            this.menuCombinacion.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItem45,
            this.menuItem16,
            this.menuItem50,
            this.verBoletosEnEditorDeTextoToolStripMenuItem,
            this.menuItem29,
            this.menuItem46,
            this.menuItem47,
            this.escrutarCombinacionesToolStripMenuItem,
            this.menuItem56,
            this.menuItem20,
            this.menuItem15,
            this.menuItem28,
            this.menuItem48,
            this.menuItem49,
            this.menuItem51});
            this.menuCombinacion.Image = ((System.Drawing.Image)(resources.GetObject("menuCombinacion.Image")));
            this.menuCombinacion.Name = "menuCombinacion";
            this.menuCombinacion.Size = new System.Drawing.Size(95, 24);
            this.menuCombinacion.Text = "Combinación";
            // 
            // menuItem45
            // 
            this.menuItem45.Image = ((System.Drawing.Image)(resources.GetObject("menuItem45.Image")));
            this.menuItem45.Name = "menuItem45";
            this.menuItem45.Size = new System.Drawing.Size(231, 22);
            this.menuItem45.Text = "Calcular";
            this.menuItem45.Click += new System.EventHandler(this.MCalcular);
            // 
            // menuItem16
            // 
            this.menuItem16.Image = ((System.Drawing.Image)(resources.GetObject("menuItem16.Image")));
            this.menuItem16.Name = "menuItem16";
            this.menuItem16.Size = new System.Drawing.Size(231, 22);
            this.menuItem16.Text = "Calcular Varias Combinaciones";
            this.menuItem16.Click += new System.EventHandler(this.MCalcularMult);
            // 
            // menuItem50
            // 
            this.menuItem50.Image = ((System.Drawing.Image)(resources.GetObject("menuItem50.Image")));
            this.menuItem50.Name = "menuItem50";
            this.menuItem50.Size = new System.Drawing.Size(231, 22);
            this.menuItem50.Text = "Ver Boletos";
            this.menuItem50.Click += new System.EventHandler(this.menuItem50_Click);
            // 
            // verBoletosEnEditorDeTextoToolStripMenuItem
            // 
            this.verBoletosEnEditorDeTextoToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("verBoletosEnEditorDeTextoToolStripMenuItem.Image")));
            this.verBoletosEnEditorDeTextoToolStripMenuItem.Name = "verBoletosEnEditorDeTextoToolStripMenuItem";
            this.verBoletosEnEditorDeTextoToolStripMenuItem.Size = new System.Drawing.Size(231, 22);
            this.verBoletosEnEditorDeTextoToolStripMenuItem.Text = "Ver Boletos en Editor de Texto";
            this.verBoletosEnEditorDeTextoToolStripMenuItem.Click += new System.EventHandler(this.verBoletosEnEditorDeTextoToolStripMenuItem_Click);
            // 
            // menuItem29
            // 
            this.menuItem29.Image = ((System.Drawing.Image)(resources.GetObject("menuItem29.Image")));
            this.menuItem29.Name = "menuItem29";
            this.menuItem29.Size = new System.Drawing.Size(231, 22);
            this.menuItem29.Text = "Imprimir en Boletos Oficiales";
            this.menuItem29.Click += new System.EventHandler(this.MAbreImprimirBoletos);
            // 
            // menuItem46
            // 
            this.menuItem46.Image = ((System.Drawing.Image)(resources.GetObject("menuItem46.Image")));
            this.menuItem46.Name = "menuItem46";
            this.menuItem46.Size = new System.Drawing.Size(231, 22);
            this.menuItem46.Text = "Reducir";
            this.menuItem46.Click += new System.EventHandler(this.MReducir);
            // 
            // menuItem47
            // 
            this.menuItem47.Image = ((System.Drawing.Image)(resources.GetObject("menuItem47.Image")));
            this.menuItem47.Name = "menuItem47";
            this.menuItem47.Size = new System.Drawing.Size(231, 22);
            this.menuItem47.Text = "Escrutar Columnas";
            this.menuItem47.Click += new System.EventHandler(this.MEscrutinio);
            // 
            // escrutarCombinacionesToolStripMenuItem
            // 
            this.escrutarCombinacionesToolStripMenuItem.Enabled = false;
            this.escrutarCombinacionesToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("escrutarCombinacionesToolStripMenuItem.Image")));
            this.escrutarCombinacionesToolStripMenuItem.Name = "escrutarCombinacionesToolStripMenuItem";
            this.escrutarCombinacionesToolStripMenuItem.Size = new System.Drawing.Size(231, 22);
            this.escrutarCombinacionesToolStripMenuItem.Text = "Escrutar combinaciones";
            this.escrutarCombinacionesToolStripMenuItem.Click += new System.EventHandler(this.MEscrutinioComb);
            // 
            // menuItem56
            // 
            this.menuItem56.Image = ((System.Drawing.Image)(resources.GetObject("menuItem56.Image")));
            this.menuItem56.Name = "menuItem56";
            this.menuItem56.Size = new System.Drawing.Size(231, 22);
            this.menuItem56.Text = "Análisis de Columnas";
            this.menuItem56.Click += new System.EventHandler(this.mAnalizarColumnas);
            // 
            // menuItem20
            // 
            this.menuItem20.Image = ((System.Drawing.Image)(resources.GetObject("menuItem20.Image")));
            this.menuItem20.Name = "menuItem20";
            this.menuItem20.Size = new System.Drawing.Size(231, 22);
            this.menuItem20.Text = "Análisis de Fallos";
            this.menuItem20.Click += new System.EventHandler(this.mAnalizaCombinacion);
            // 
            // menuItem15
            // 
            this.menuItem15.Image = ((System.Drawing.Image)(resources.GetObject("menuItem15.Image")));
            this.menuItem15.Name = "menuItem15";
            this.menuItem15.Size = new System.Drawing.Size(231, 22);
            this.menuItem15.Text = "Análisis Gráfico";
            this.menuItem15.Click += new System.EventHandler(this.MAbreGraficoCombinacion);
            // 
            // menuItem28
            // 
            this.menuItem28.Image = ((System.Drawing.Image)(resources.GetObject("menuItem28.Image")));
            this.menuItem28.Name = "menuItem28";
            this.menuItem28.Size = new System.Drawing.Size(231, 22);
            this.menuItem28.Text = "Análisis de Signos";
            this.menuItem28.Click += new System.EventHandler(this.MAnalisisSignos);
            // 
            // menuItem48
            // 
            this.menuItem48.Image = ((System.Drawing.Image)(resources.GetObject("menuItem48.Image")));
            this.menuItem48.Name = "menuItem48";
            this.menuItem48.Size = new System.Drawing.Size(231, 22);
            this.menuItem48.Text = "Probabilidades";
            this.menuItem48.Click += new System.EventHandler(this.MProbabilidades);
            // 
            // menuItem49
            // 
            this.menuItem49.Image = ((System.Drawing.Image)(resources.GetObject("menuItem49.Image")));
            this.menuItem49.Name = "menuItem49";
            this.menuItem49.Size = new System.Drawing.Size(231, 22);
            this.menuItem49.Text = "Estadísticas";
            this.menuItem49.Click += new System.EventHandler(this.MEstadisticas);
            // 
            // menuItem51
            // 
            this.menuItem51.Image = ((System.Drawing.Image)(resources.GetObject("menuItem51.Image")));
            this.menuItem51.Name = "menuItem51";
            this.menuItem51.Size = new System.Drawing.Size(231, 22);
            this.menuItem51.Text = "Añadir P15";
            this.menuItem51.Click += new System.EventHandler(this.MAbrePonerP15);
            // 
            // menuGrupo
            // 
            this.menuGrupo.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItem22,
            this.menuItem30,
            this.menuItem27,
            this.mPegarGrupo,
            this.menuInsertarGrupo,
            this.menuItem37});
            this.menuGrupo.Image = ((System.Drawing.Image)(resources.GetObject("menuGrupo.Image")));
            this.menuGrupo.Name = "menuGrupo";
            this.menuGrupo.Size = new System.Drawing.Size(69, 24);
            this.menuGrupo.Text = "Grupos";
            this.menuGrupo.Visible = false;
            // 
            // menuItem22
            // 
            this.menuItem22.Image = ((System.Drawing.Image)(resources.GetObject("menuItem22.Image")));
            this.menuItem22.Name = "menuItem22";
            this.menuItem22.Size = new System.Drawing.Size(124, 22);
            this.menuItem22.Text = "Abrir";
            this.menuItem22.Click += new System.EventHandler(this.mAbrirGrupos);
            // 
            // menuItem30
            // 
            this.menuItem30.Image = ((System.Drawing.Image)(resources.GetObject("menuItem30.Image")));
            this.menuItem30.Name = "menuItem30";
            this.menuItem30.Size = new System.Drawing.Size(124, 22);
            this.menuItem30.Text = "Guardar";
            this.menuItem30.Click += new System.EventHandler(this.mGuardarGrupos);
            // 
            // menuItem27
            // 
            this.menuItem27.Image = ((System.Drawing.Image)(resources.GetObject("menuItem27.Image")));
            this.menuItem27.Name = "menuItem27";
            this.menuItem27.Size = new System.Drawing.Size(124, 22);
            this.menuItem27.Text = "Copiar";
            this.menuItem27.Click += new System.EventHandler(this.mCopiarGrupos);
            // 
            // mPegarGrupo
            // 
            this.mPegarGrupo.Enabled = false;
            this.mPegarGrupo.Image = ((System.Drawing.Image)(resources.GetObject("mPegarGrupo.Image")));
            this.mPegarGrupo.Name = "mPegarGrupo";
            this.mPegarGrupo.Size = new System.Drawing.Size(124, 22);
            this.mPegarGrupo.Text = "Pegar";
            this.mPegarGrupo.Click += new System.EventHandler(this.mPegarGrupos);
            // 
            // menuInsertarGrupo
            // 
            this.menuInsertarGrupo.Image = ((System.Drawing.Image)(resources.GetObject("menuInsertarGrupo.Image")));
            this.menuInsertarGrupo.Name = "menuInsertarGrupo";
            this.menuInsertarGrupo.Size = new System.Drawing.Size(124, 22);
            this.menuInsertarGrupo.Text = "Insertar";
            this.menuInsertarGrupo.Click += new System.EventHandler(this.mInsertarGrupos);
            // 
            // menuItem37
            // 
            this.menuItem37.Image = ((System.Drawing.Image)(resources.GetObject("menuItem37.Image")));
            this.menuItem37.Name = "menuItem37";
            this.menuItem37.Size = new System.Drawing.Size(124, 22);
            this.menuItem37.Text = "Eliminar";
            this.menuItem37.Click += new System.EventHandler(this.mEliminarGrupos);
            // 
            // menuFiltros
            // 
            this.menuFiltros.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItem13,
            this.menuItem17,
            this.menuItem23,
            this.mFiltroAidomnou,
            this.mFiltroPim});
            this.menuFiltros.Image = ((System.Drawing.Image)(resources.GetObject("menuFiltros.Image")));
            this.menuFiltros.Name = "menuFiltros";
            this.menuFiltros.Size = new System.Drawing.Size(64, 24);
            this.menuFiltros.Text = "Filtros";
            // 
            // menuItem13
            // 
            this.menuItem13.Image = ((System.Drawing.Image)(resources.GetObject("menuItem13.Image")));
            this.menuItem13.Name = "menuItem13";
            this.menuItem13.Size = new System.Drawing.Size(199, 22);
            this.menuItem13.Text = "Combinar Filtros";
            this.menuItem13.Click += new System.EventHandler(this.MAbreFiltros);
            // 
            // menuItem17
            // 
            this.menuItem17.Image = ((System.Drawing.Image)(resources.GetObject("menuItem17.Image")));
            this.menuItem17.Name = "menuItem17";
            this.menuItem17.Size = new System.Drawing.Size(199, 22);
            this.menuItem17.Text = "Diferencias entre Filtros";
            this.menuItem17.Click += new System.EventHandler(this.MDifEntreFiltros);
            // 
            // menuItem23
            // 
            this.menuItem23.Image = ((System.Drawing.Image)(resources.GetObject("menuItem23.Image")));
            this.menuItem23.Name = "menuItem23";
            this.menuItem23.Size = new System.Drawing.Size(199, 22);
            this.menuItem23.Text = "Filtro Coincidencias";
            this.menuItem23.Click += new System.EventHandler(this.MAbreFiltroCoincidencias);
            // 
            // mFiltroAidomnou
            // 
            this.mFiltroAidomnou.Image = ((System.Drawing.Image)(resources.GetObject("mFiltroAidomnou.Image")));
            this.mFiltroAidomnou.Name = "mFiltroAidomnou";
            this.mFiltroAidomnou.Size = new System.Drawing.Size(199, 22);
            this.mFiltroAidomnou.Text = "Filtro \"Aidomnou\"";
            this.mFiltroAidomnou.Click += new System.EventHandler(this.mFiltroAidomnou_Click);
            // 
            // mFiltroPim
            // 
            this.mFiltroPim.Image = ((System.Drawing.Image)(resources.GetObject("mFiltroPim.Image")));
            this.mFiltroPim.Name = "mFiltroPim";
            this.mFiltroPim.Size = new System.Drawing.Size(199, 22);
            this.mFiltroPim.Text = "Filtro \"Pim\"";
            this.mFiltroPim.Click += new System.EventHandler(this.mFiltroPim_Click);
            // 
            // menuOperaciones
            // 
            this.menuOperaciones.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItem6,
            this.menuItem31,
            this.menuItem32,
            this.menuItem33,
            this.menuItem34});
            this.menuOperaciones.Image = ((System.Drawing.Image)(resources.GetObject("menuOperaciones.Image")));
            this.menuOperaciones.Name = "menuOperaciones";
            this.menuOperaciones.Size = new System.Drawing.Size(95, 24);
            this.menuOperaciones.Text = "Operaciones";
            // 
            // menuItem6
            // 
            this.menuItem6.Image = ((System.Drawing.Image)(resources.GetObject("menuItem6.Image")));
            this.menuItem6.Name = "menuItem6";
            this.menuItem6.Size = new System.Drawing.Size(176, 22);
            this.menuItem6.Text = "Algebra";
            this.menuItem6.Click += new System.EventHandler(this.MCombSumar);
            // 
            // menuItem31
            // 
            this.menuItem31.Image = ((System.Drawing.Image)(resources.GetObject("menuItem31.Image")));
            this.menuItem31.Name = "menuItem31";
            this.menuItem31.Size = new System.Drawing.Size(176, 22);
            this.menuItem31.Text = "Transposición";
            this.menuItem31.Click += new System.EventHandler(this.MTransposicionFrm);
            // 
            // menuItem32
            // 
            this.menuItem32.Image = ((System.Drawing.Image)(resources.GetObject("menuItem32.Image")));
            this.menuItem32.Name = "menuItem32";
            this.menuItem32.Size = new System.Drawing.Size(176, 22);
            this.menuItem32.Text = "Multiplicador";
            this.menuItem32.Click += new System.EventHandler(this.MMultiplicador);
            // 
            // menuItem33
            // 
            this.menuItem33.Image = ((System.Drawing.Image)(resources.GetObject("menuItem33.Image")));
            this.menuItem33.Name = "menuItem33";
            this.menuItem33.Size = new System.Drawing.Size(176, 22);
            this.menuItem33.Text = "Fraccionador";
            this.menuItem33.Click += new System.EventHandler(this.MFraccionador);
            // 
            // menuItem34
            // 
            this.menuItem34.Image = ((System.Drawing.Image)(resources.GetObject("menuItem34.Image")));
            this.menuItem34.Name = "menuItem34";
            this.menuItem34.Size = new System.Drawing.Size(176, 22);
            this.menuItem34.Text = "Rotación de Signos";
            this.menuItem34.Click += new System.EventHandler(this.MRotacionDeSignos);
            // 
            // menuUtilidades
            // 
            this.menuUtilidades.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItem10,
            this.menuItem11,
            this.menuItem12,
            this.menuItem14,
            this.menuItem18,
            this.menuItem36,
            this.menuItem19,
            this.menuItem21,
            this.menuItem24,
            this.menuItem25,
            this.menuItem26,
            this.menuItem35,
            this.menuItem38,
            this.menuItem55,
            this.menuItem54,
            this.MnuReduccionesPerfectas,
            this.menuDepLineal,
            this.compresorToolStripMenuItem,
            this.estuColToolStripMenuItem});
            this.menuUtilidades.Image = ((System.Drawing.Image)(resources.GetObject("menuUtilidades.Image")));
            this.menuUtilidades.Name = "menuUtilidades";
            this.menuUtilidades.Size = new System.Drawing.Size(81, 24);
            this.menuUtilidades.Text = "Utilidades";
            // 
            // menuItem10
            // 
            this.menuItem10.Image = ((System.Drawing.Image)(resources.GetObject("menuItem10.Image")));
            this.menuItem10.Name = "menuItem10";
            this.menuItem10.Size = new System.Drawing.Size(250, 22);
            this.menuItem10.Text = "Sube Categoría";
            this.menuItem10.Click += new System.EventHandler(this.MSubeCategoria);
            // 
            // menuItem11
            // 
            this.menuItem11.Image = ((System.Drawing.Image)(resources.GetObject("menuItem11.Image")));
            this.menuItem11.Name = "menuItem11";
            this.menuItem11.Size = new System.Drawing.Size(250, 22);
            this.menuItem11.Text = "Modificador %";
            this.menuItem11.Click += new System.EventHandler(this.MAbreModificador);
            // 
            // menuItem12
            // 
            this.menuItem12.Image = ((System.Drawing.Image)(resources.GetObject("menuItem12.Image")));
            this.menuItem12.Name = "menuItem12";
            this.menuItem12.Size = new System.Drawing.Size(250, 22);
            this.menuItem12.Text = "Generador Columnas Probables";
            this.menuItem12.Click += new System.EventHandler(this.MGeneradorCol);
            // 
            // menuItem14
            // 
            this.menuItem14.Image = ((System.Drawing.Image)(resources.GetObject("menuItem14.Image")));
            this.menuItem14.Name = "menuItem14";
            this.menuItem14.Size = new System.Drawing.Size(250, 22);
            this.menuItem14.Text = "Diferencias entre Columnas";
            this.menuItem14.Click += new System.EventHandler(this.MDifEntreColumnas);
            // 
            // menuItem18
            // 
            this.menuItem18.Image = ((System.Drawing.Image)(resources.GetObject("menuItem18.Image")));
            this.menuItem18.Name = "menuItem18";
            this.menuItem18.Size = new System.Drawing.Size(250, 22);
            this.menuItem18.Text = "Ordenar por Probabilidad";
            this.menuItem18.Click += new System.EventHandler(this.MProbabilidad);
            // 
            // menuItem36
            // 
            this.menuItem36.Image = ((System.Drawing.Image)(resources.GetObject("menuItem36.Image")));
            this.menuItem36.Name = "menuItem36";
            this.menuItem36.Size = new System.Drawing.Size(250, 22);
            this.menuItem36.Text = "Selector (JuanM)";
            this.menuItem36.Click += new System.EventHandler(this.MAbreSelectorJM);
            // 
            // menuItem19
            // 
            this.menuItem19.Image = ((System.Drawing.Image)(resources.GetObject("menuItem19.Image")));
            this.menuItem19.Name = "menuItem19";
            this.menuItem19.Size = new System.Drawing.Size(250, 22);
            this.menuItem19.Text = "Selector (MarioSan)";
            this.menuItem19.Click += new System.EventHandler(this.MAbreSelectorMS);
            // 
            // menuItem21
            // 
            this.menuItem21.Image = ((System.Drawing.Image)(resources.GetObject("menuItem21.Image")));
            this.menuItem21.Name = "menuItem21";
            this.menuItem21.Size = new System.Drawing.Size(250, 22);
            this.menuItem21.Text = "Rentabilidad";
            this.menuItem21.Click += new System.EventHandler(this.MAbreRentabilidad);
            // 
            // menuItem24
            // 
            this.menuItem24.Image = ((System.Drawing.Image)(resources.GetObject("menuItem24.Image")));
            this.menuItem24.Name = "menuItem24";
            this.menuItem24.Size = new System.Drawing.Size(250, 22);
            this.menuItem24.Text = "Columnas GEPT";
            this.menuItem24.Click += new System.EventHandler(this.MAbreColumnasGEPT);
            // 
            // menuItem25
            // 
            this.menuItem25.Image = ((System.Drawing.Image)(resources.GetObject("menuItem25.Image")));
            this.menuItem25.Name = "menuItem25";
            this.menuItem25.Size = new System.Drawing.Size(250, 22);
            this.menuItem25.Text = "Tramificar";
            this.menuItem25.Click += new System.EventHandler(this.MAbreSeleccionTramos);
            // 
            // menuItem26
            // 
            this.menuItem26.Image = ((System.Drawing.Image)(resources.GetObject("menuItem26.Image")));
            this.menuItem26.Name = "menuItem26";
            this.menuItem26.Size = new System.Drawing.Size(250, 22);
            this.menuItem26.Text = "Premiadas";
            this.menuItem26.Click += new System.EventHandler(this.MAbreSelectorPremiadas);
            // 
            // menuItem35
            // 
            this.menuItem35.Image = ((System.Drawing.Image)(resources.GetObject("menuItem35.Image")));
            this.menuItem35.Name = "menuItem35";
            this.menuItem35.Size = new System.Drawing.Size(250, 22);
            this.menuItem35.Text = "Estimación de Premios";
            this.menuItem35.Click += new System.EventHandler(this.MEstimacion);
            // 
            // menuItem38
            // 
            this.menuItem38.Image = ((System.Drawing.Image)(resources.GetObject("menuItem38.Image")));
            this.menuItem38.Name = "menuItem38";
            this.menuItem38.Size = new System.Drawing.Size(250, 22);
            this.menuItem38.Text = "Simulador de Escrutinios";
            this.menuItem38.Click += new System.EventHandler(this.MBancoPruebas);
            // 
            // menuItem55
            // 
            this.menuItem55.Image = ((System.Drawing.Image)(resources.GetObject("menuItem55.Image")));
            this.menuItem55.Name = "menuItem55";
            this.menuItem55.Size = new System.Drawing.Size(250, 22);
            this.menuItem55.Text = "Importar/Exportar";
            this.menuItem55.Click += new System.EventHandler(this.MImportExport);
            // 
            // menuItem54
            // 
            this.menuItem54.Image = ((System.Drawing.Image)(resources.GetObject("menuItem54.Image")));
            this.menuItem54.Name = "menuItem54";
            this.menuItem54.Size = new System.Drawing.Size(250, 22);
            this.menuItem54.Text = "Análisis de Grupos en Combinación";
            this.menuItem54.Click += new System.EventHandler(this.menuItem54_Click);
            // 
            // MnuReduccionesPerfectas
            // 
            this.MnuReduccionesPerfectas.Image = ((System.Drawing.Image)(resources.GetObject("MnuReduccionesPerfectas.Image")));
            this.MnuReduccionesPerfectas.Name = "MnuReduccionesPerfectas";
            this.MnuReduccionesPerfectas.Size = new System.Drawing.Size(250, 22);
            this.MnuReduccionesPerfectas.Text = "Reducciones Perfectas";
            this.MnuReduccionesPerfectas.Click += new System.EventHandler(this.MnuReduccionesPerfectas_Click);
            // 
            // menuDepLineal
            // 
            this.menuDepLineal.Image = ((System.Drawing.Image)(resources.GetObject("menuDepLineal.Image")));
            this.menuDepLineal.Name = "menuDepLineal";
            this.menuDepLineal.Size = new System.Drawing.Size(250, 22);
            this.menuDepLineal.Text = "Dependencia Lineal";
            this.menuDepLineal.Click += new System.EventHandler(this.MDepLineal);
            // 
            // compresorToolStripMenuItem
            // 
            this.compresorToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("compresorToolStripMenuItem.Image")));
            this.compresorToolStripMenuItem.Name = "compresorToolStripMenuItem";
            this.compresorToolStripMenuItem.Size = new System.Drawing.Size(250, 22);
            this.compresorToolStripMenuItem.Text = "Compresor z3q";
            this.compresorToolStripMenuItem.Click += new System.EventHandler(this.compresorToolStripMenuItem_Click);
            // 
            // estuColToolStripMenuItem
            // 
            this.estuColToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("estuColToolStripMenuItem.Image")));
            this.estuColToolStripMenuItem.Name = "estuColToolStripMenuItem";
            this.estuColToolStripMenuItem.Size = new System.Drawing.Size(250, 22);
            this.estuColToolStripMenuItem.Text = "EstuCol";
            this.estuColToolStripMenuItem.Click += new System.EventHandler(this.estuColToolStripMenuItem_Click);
            // 
            // tsFree
            // 
            this.tsFree.Dock = System.Windows.Forms.DockStyle.None;
            this.tsFree.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.bSalir,
            this.bConfig,
            this.bConfAnalisis,
            this.bAyuda,
            this.bAcercaDe});
            this.tsFree.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
            this.tsFree.Location = new System.Drawing.Point(3, 0);
            this.tsFree.Name = "tsFree";
            this.tsFree.Size = new System.Drawing.Size(127, 25);
            this.tsFree.TabIndex = 0;
            this.tsFree.Visible = false;
            // 
            // bSalir
            // 
            this.bSalir.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bSalir.Image = ((System.Drawing.Image)(resources.GetObject("bSalir.Image")));
            this.bSalir.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.bSalir.Name = "bSalir";
            this.bSalir.Size = new System.Drawing.Size(23, 22);
            this.bSalir.Text = "toolStripButton1";
            this.bSalir.ToolTipText = "Salir";
            this.bSalir.Click += new System.EventHandler(this.MSalir);
            // 
            // bConfig
            // 
            this.bConfig.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bConfig.Image = ((System.Drawing.Image)(resources.GetObject("bConfig.Image")));
            this.bConfig.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.bConfig.Name = "bConfig";
            this.bConfig.Size = new System.Drawing.Size(23, 22);
            this.bConfig.Text = "toolStripButton2";
            this.bConfig.ToolTipText = "Configuración";
            this.bConfig.Click += new System.EventHandler(this.MConfiguracion);
            // 
            // bConfAnalisis
            // 
            this.bConfAnalisis.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bConfAnalisis.Image = ((System.Drawing.Image)(resources.GetObject("bConfAnalisis.Image")));
            this.bConfAnalisis.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.bConfAnalisis.Name = "bConfAnalisis";
            this.bConfAnalisis.Size = new System.Drawing.Size(23, 22);
            this.bConfAnalisis.Text = "toolStripButton1";
            this.bConfAnalisis.ToolTipText = "Configurar Análisis";
            this.bConfAnalisis.Click += new System.EventHandler(this.bConfAnalisis_Click);
            // 
            // bAyuda
            // 
            this.bAyuda.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bAyuda.Image = ((System.Drawing.Image)(resources.GetObject("bAyuda.Image")));
            this.bAyuda.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.bAyuda.Name = "bAyuda";
            this.bAyuda.Size = new System.Drawing.Size(23, 22);
            this.bAyuda.Text = "toolStripButton1";
            this.bAyuda.ToolTipText = "Ayuda";
            this.bAyuda.Click += new System.EventHandler(this.MAyuda);
            // 
            // bAcercaDe
            // 
            this.bAcercaDe.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bAcercaDe.Image = ((System.Drawing.Image)(resources.GetObject("bAcercaDe.Image")));
            this.bAcercaDe.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.bAcercaDe.Name = "bAcercaDe";
            this.bAcercaDe.Size = new System.Drawing.Size(23, 22);
            this.bAcercaDe.Text = "toolStripButton3";
            this.bAcercaDe.ToolTipText = "Acerca de ...";
            this.bAcercaDe.Click += new System.EventHandler(this.mAcercaDe);
            // 
            // tsArchivo
            // 
            this.tsArchivo.Dock = System.Windows.Forms.DockStyle.None;
            this.tsArchivo.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.bGuardarEquipos,
            this.bNuevo,
            this.bObtenerBoletos,
            this.bAbrirCombinacion,
            this.bGuardarCombinacion,
            this.bGuardarCombinacionComo,
            this.bBorrarTemporales,
            this.bAbrirEquipos,
            this.bBorrarInformes,
            this.bGestorEquipos});
            this.tsArchivo.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
            this.tsArchivo.Location = new System.Drawing.Point(185, 0);
            this.tsArchivo.Name = "tsArchivo";
            this.tsArchivo.Size = new System.Drawing.Size(242, 25);
            this.tsArchivo.TabIndex = 3;
            this.tsArchivo.Visible = false;
            // 
            // bGuardarEquipos
            // 
            this.bGuardarEquipos.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bGuardarEquipos.Image = ((System.Drawing.Image)(resources.GetObject("bGuardarEquipos.Image")));
            this.bGuardarEquipos.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.bGuardarEquipos.Name = "bGuardarEquipos";
            this.bGuardarEquipos.Size = new System.Drawing.Size(23, 22);
            this.bGuardarEquipos.Text = "toolStripButton2";
            this.bGuardarEquipos.ToolTipText = "Guardar equipos";
            this.bGuardarEquipos.Click += new System.EventHandler(this.MGuardarPartidosClick);
            // 
            // bNuevo
            // 
            this.bNuevo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bNuevo.Image = ((System.Drawing.Image)(resources.GetObject("bNuevo.Image")));
            this.bNuevo.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.bNuevo.Name = "bNuevo";
            this.bNuevo.Size = new System.Drawing.Size(23, 22);
            this.bNuevo.Text = "toolStripButton3";
            this.bNuevo.ToolTipText = "Nueva combinación";
            this.bNuevo.Click += new System.EventHandler(this.MNuevaComb);
            // 
            // bObtenerBoletos
            // 
            this.bObtenerBoletos.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bObtenerBoletos.Image = ((System.Drawing.Image)(resources.GetObject("bObtenerBoletos.Image")));
            this.bObtenerBoletos.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.bObtenerBoletos.Name = "bObtenerBoletos";
            this.bObtenerBoletos.Size = new System.Drawing.Size(23, 22);
            this.bObtenerBoletos.Text = "toolStripButton1";
            this.bObtenerBoletos.ToolTipText = "Obtener Boletos Online";
            this.bObtenerBoletos.Click += new System.EventHandler(this.obtenerBoletosOnlineToolStripMenuItem_Click);
            // 
            // bAbrirCombinacion
            // 
            this.bAbrirCombinacion.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bAbrirCombinacion.Image = ((System.Drawing.Image)(resources.GetObject("bAbrirCombinacion.Image")));
            this.bAbrirCombinacion.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.bAbrirCombinacion.Name = "bAbrirCombinacion";
            this.bAbrirCombinacion.Size = new System.Drawing.Size(23, 22);
            this.bAbrirCombinacion.Text = "toolStripButton4";
            this.bAbrirCombinacion.ToolTipText = "Abrir combinación";
            this.bAbrirCombinacion.Click += new System.EventHandler(this.MAbrirCombClick);
            // 
            // bGuardarCombinacion
            // 
            this.bGuardarCombinacion.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bGuardarCombinacion.Image = ((System.Drawing.Image)(resources.GetObject("bGuardarCombinacion.Image")));
            this.bGuardarCombinacion.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.bGuardarCombinacion.Name = "bGuardarCombinacion";
            this.bGuardarCombinacion.Size = new System.Drawing.Size(23, 22);
            this.bGuardarCombinacion.Text = "toolStripButton5";
            this.bGuardarCombinacion.ToolTipText = "Guardar combinación";
            this.bGuardarCombinacion.Click += new System.EventHandler(this.MGuardarComb);
            // 
            // bGuardarCombinacionComo
            // 
            this.bGuardarCombinacionComo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bGuardarCombinacionComo.Image = ((System.Drawing.Image)(resources.GetObject("bGuardarCombinacionComo.Image")));
            this.bGuardarCombinacionComo.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.bGuardarCombinacionComo.Name = "bGuardarCombinacionComo";
            this.bGuardarCombinacionComo.Size = new System.Drawing.Size(23, 22);
            this.bGuardarCombinacionComo.Text = "toolStripButton1";
            this.bGuardarCombinacionComo.ToolTipText = "Guardar combinación como";
            this.bGuardarCombinacionComo.Click += new System.EventHandler(this.MGuardarCombComo);
            // 
            // bBorrarTemporales
            // 
            this.bBorrarTemporales.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bBorrarTemporales.Image = ((System.Drawing.Image)(resources.GetObject("bBorrarTemporales.Image")));
            this.bBorrarTemporales.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.bBorrarTemporales.Name = "bBorrarTemporales";
            this.bBorrarTemporales.Size = new System.Drawing.Size(23, 22);
            this.bBorrarTemporales.Text = "toolStripButton1";
            this.bBorrarTemporales.ToolTipText = "Borrar archivos temporales";
            this.bBorrarTemporales.Click += new System.EventHandler(this.MBorrarCombsTemp);
            // 
            // bAbrirEquipos
            // 
            this.bAbrirEquipos.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bAbrirEquipos.Image = ((System.Drawing.Image)(resources.GetObject("bAbrirEquipos.Image")));
            this.bAbrirEquipos.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.bAbrirEquipos.Name = "bAbrirEquipos";
            this.bAbrirEquipos.Size = new System.Drawing.Size(23, 22);
            this.bAbrirEquipos.Text = "toolStripButton1";
            this.bAbrirEquipos.ToolTipText = "Abrir equipos";
            this.bAbrirEquipos.Click += new System.EventHandler(this.MAbreBoleto);
            // 
            // bBorrarInformes
            // 
            this.bBorrarInformes.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bBorrarInformes.Image = ((System.Drawing.Image)(resources.GetObject("bBorrarInformes.Image")));
            this.bBorrarInformes.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.bBorrarInformes.Name = "bBorrarInformes";
            this.bBorrarInformes.Size = new System.Drawing.Size(23, 22);
            this.bBorrarInformes.Text = "toolStripButton1";
            this.bBorrarInformes.ToolTipText = "Borrar Informes de Error";
            this.bBorrarInformes.Click += new System.EventHandler(this.borrarInformesDeErrorToolStripMenuItem_Click);
            // 
            // bGestorEquipos
            // 
            this.bGestorEquipos.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bGestorEquipos.Image = ((System.Drawing.Image)(resources.GetObject("bGestorEquipos.Image")));
            this.bGestorEquipos.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.bGestorEquipos.Name = "bGestorEquipos";
            this.bGestorEquipos.Size = new System.Drawing.Size(23, 22);
            this.bGestorEquipos.Text = "toolStripButton1";
            this.bGestorEquipos.ToolTipText = "Gestión de Equipos";
            this.bGestorEquipos.Click += new System.EventHandler(this.gestiónDeEquiposToolStripMenuItem_Click);
            // 
            // tsOperaciones
            // 
            this.tsOperaciones.Dock = System.Windows.Forms.DockStyle.None;
            this.tsOperaciones.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.bAlgebra,
            this.bTransposición,
            this.bMultiplicacion,
            this.bFraccionador,
            this.bRotacion});
            this.tsOperaciones.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
            this.tsOperaciones.Location = new System.Drawing.Point(3, 0);
            this.tsOperaciones.Name = "tsOperaciones";
            this.tsOperaciones.Size = new System.Drawing.Size(127, 25);
            this.tsOperaciones.TabIndex = 5;
            this.tsOperaciones.Visible = false;
            // 
            // bAlgebra
            // 
            this.bAlgebra.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bAlgebra.Image = ((System.Drawing.Image)(resources.GetObject("bAlgebra.Image")));
            this.bAlgebra.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.bAlgebra.Name = "bAlgebra";
            this.bAlgebra.Size = new System.Drawing.Size(23, 22);
            this.bAlgebra.Text = "toolStripButton1";
            this.bAlgebra.ToolTipText = "Algebra";
            this.bAlgebra.Click += new System.EventHandler(this.MCombSumar);
            // 
            // bTransposición
            // 
            this.bTransposición.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bTransposición.Image = ((System.Drawing.Image)(resources.GetObject("bTransposición.Image")));
            this.bTransposición.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.bTransposición.Name = "bTransposición";
            this.bTransposición.Size = new System.Drawing.Size(23, 22);
            this.bTransposición.Text = "toolStripButton2";
            this.bTransposición.ToolTipText = "Transposición";
            this.bTransposición.Click += new System.EventHandler(this.MTransposicionFrm);
            // 
            // bMultiplicacion
            // 
            this.bMultiplicacion.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bMultiplicacion.Image = ((System.Drawing.Image)(resources.GetObject("bMultiplicacion.Image")));
            this.bMultiplicacion.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.bMultiplicacion.Name = "bMultiplicacion";
            this.bMultiplicacion.Size = new System.Drawing.Size(23, 22);
            this.bMultiplicacion.Text = "toolStripButton3";
            this.bMultiplicacion.ToolTipText = "Multiplicación";
            this.bMultiplicacion.Click += new System.EventHandler(this.MMultiplicador);
            // 
            // bFraccionador
            // 
            this.bFraccionador.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bFraccionador.Image = ((System.Drawing.Image)(resources.GetObject("bFraccionador.Image")));
            this.bFraccionador.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.bFraccionador.Name = "bFraccionador";
            this.bFraccionador.Size = new System.Drawing.Size(23, 22);
            this.bFraccionador.Text = "toolStripButton4";
            this.bFraccionador.ToolTipText = "Fraccionar";
            this.bFraccionador.Click += new System.EventHandler(this.MFraccionador);
            // 
            // bRotacion
            // 
            this.bRotacion.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bRotacion.Image = ((System.Drawing.Image)(resources.GetObject("bRotacion.Image")));
            this.bRotacion.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.bRotacion.Name = "bRotacion";
            this.bRotacion.Size = new System.Drawing.Size(23, 22);
            this.bRotacion.Text = "toolStripButton5";
            this.bRotacion.ToolTipText = "Rotación de signos";
            this.bRotacion.Click += new System.EventHandler(this.MRotacionDeSignos);
            // 
            // tsUtilidades
            // 
            this.tsUtilidades.Dock = System.Windows.Forms.DockStyle.None;
            this.tsUtilidades.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.bSubeCategoria,
            this.bModificadorPct,
            this.bGeneradorCPs,
            this.bDiferenciasColumnas,
            this.bProbabilidad,
            this.bSelectorJuanM,
            this.bSelectorMarioSan,
            this.bRentabilidad,
            this.bColumnasGEPT,
            this.bTramificar,
            this.bPremiadas,
            this.bEstimacion,
            this.bBancoPruebas,
            this.bImportExport,
            this.bAnalisisGrupos,
            this.bRedPerfectas,
            this.bDependenciaLineal});
            this.tsUtilidades.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
            this.tsUtilidades.Location = new System.Drawing.Point(337, 0);
            this.tsUtilidades.Name = "tsUtilidades";
            this.tsUtilidades.Size = new System.Drawing.Size(403, 25);
            this.tsUtilidades.TabIndex = 2;
            this.tsUtilidades.Visible = false;
            // 
            // bSubeCategoria
            // 
            this.bSubeCategoria.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bSubeCategoria.Image = ((System.Drawing.Image)(resources.GetObject("bSubeCategoria.Image")));
            this.bSubeCategoria.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.bSubeCategoria.Name = "bSubeCategoria";
            this.bSubeCategoria.Size = new System.Drawing.Size(23, 22);
            this.bSubeCategoria.Text = "toolStripButton1";
            this.bSubeCategoria.ToolTipText = "Subir categoría";
            this.bSubeCategoria.Click += new System.EventHandler(this.MSubeCategoria);
            // 
            // bModificadorPct
            // 
            this.bModificadorPct.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bModificadorPct.Image = ((System.Drawing.Image)(resources.GetObject("bModificadorPct.Image")));
            this.bModificadorPct.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.bModificadorPct.Name = "bModificadorPct";
            this.bModificadorPct.Size = new System.Drawing.Size(23, 22);
            this.bModificadorPct.Text = "toolStripButton1";
            this.bModificadorPct.ToolTipText = "Modificador de porcentajes";
            this.bModificadorPct.Click += new System.EventHandler(this.MAbreModificador);
            // 
            // bGeneradorCPs
            // 
            this.bGeneradorCPs.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bGeneradorCPs.Image = ((System.Drawing.Image)(resources.GetObject("bGeneradorCPs.Image")));
            this.bGeneradorCPs.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.bGeneradorCPs.Name = "bGeneradorCPs";
            this.bGeneradorCPs.Size = new System.Drawing.Size(23, 22);
            this.bGeneradorCPs.Text = "toolStripButton2";
            this.bGeneradorCPs.ToolTipText = "Generador de CPs";
            this.bGeneradorCPs.Click += new System.EventHandler(this.MGeneradorCol);
            // 
            // bDiferenciasColumnas
            // 
            this.bDiferenciasColumnas.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bDiferenciasColumnas.Image = ((System.Drawing.Image)(resources.GetObject("bDiferenciasColumnas.Image")));
            this.bDiferenciasColumnas.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.bDiferenciasColumnas.Name = "bDiferenciasColumnas";
            this.bDiferenciasColumnas.Size = new System.Drawing.Size(23, 22);
            this.bDiferenciasColumnas.Text = "toolStripButton3";
            this.bDiferenciasColumnas.ToolTipText = "Diferencias entre columnas";
            this.bDiferenciasColumnas.Click += new System.EventHandler(this.MDifEntreColumnas);
            // 
            // bProbabilidad
            // 
            this.bProbabilidad.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bProbabilidad.Image = ((System.Drawing.Image)(resources.GetObject("bProbabilidad.Image")));
            this.bProbabilidad.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.bProbabilidad.Name = "bProbabilidad";
            this.bProbabilidad.Size = new System.Drawing.Size(23, 22);
            this.bProbabilidad.Text = "toolStripButton4";
            this.bProbabilidad.ToolTipText = "Ordenar por probabilidad";
            this.bProbabilidad.Click += new System.EventHandler(this.MProbabilidad);
            // 
            // bSelectorJuanM
            // 
            this.bSelectorJuanM.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bSelectorJuanM.Image = ((System.Drawing.Image)(resources.GetObject("bSelectorJuanM.Image")));
            this.bSelectorJuanM.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.bSelectorJuanM.Name = "bSelectorJuanM";
            this.bSelectorJuanM.Size = new System.Drawing.Size(23, 22);
            this.bSelectorJuanM.Text = "toolStripButton5";
            this.bSelectorJuanM.ToolTipText = "Selector JuanM";
            this.bSelectorJuanM.Click += new System.EventHandler(this.MAbreSelectorJM);
            // 
            // bSelectorMarioSan
            // 
            this.bSelectorMarioSan.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bSelectorMarioSan.Image = ((System.Drawing.Image)(resources.GetObject("bSelectorMarioSan.Image")));
            this.bSelectorMarioSan.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.bSelectorMarioSan.Name = "bSelectorMarioSan";
            this.bSelectorMarioSan.Size = new System.Drawing.Size(23, 22);
            this.bSelectorMarioSan.Text = "toolStripButton6";
            this.bSelectorMarioSan.ToolTipText = "Selector MarioSan";
            this.bSelectorMarioSan.Click += new System.EventHandler(this.MAbreSelectorMS);
            // 
            // bRentabilidad
            // 
            this.bRentabilidad.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bRentabilidad.Image = ((System.Drawing.Image)(resources.GetObject("bRentabilidad.Image")));
            this.bRentabilidad.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.bRentabilidad.Name = "bRentabilidad";
            this.bRentabilidad.Size = new System.Drawing.Size(23, 22);
            this.bRentabilidad.Text = "toolStripButton7";
            this.bRentabilidad.ToolTipText = "Rentabilidad";
            this.bRentabilidad.Click += new System.EventHandler(this.MAbreRentabilidad);
            // 
            // bColumnasGEPT
            // 
            this.bColumnasGEPT.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bColumnasGEPT.Image = ((System.Drawing.Image)(resources.GetObject("bColumnasGEPT.Image")));
            this.bColumnasGEPT.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.bColumnasGEPT.Name = "bColumnasGEPT";
            this.bColumnasGEPT.Size = new System.Drawing.Size(23, 22);
            this.bColumnasGEPT.Text = "toolStripButton8";
            this.bColumnasGEPT.ToolTipText = "Columnas GEPT";
            this.bColumnasGEPT.Click += new System.EventHandler(this.MAbreColumnasGEPT);
            // 
            // bTramificar
            // 
            this.bTramificar.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bTramificar.Image = ((System.Drawing.Image)(resources.GetObject("bTramificar.Image")));
            this.bTramificar.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.bTramificar.Name = "bTramificar";
            this.bTramificar.Size = new System.Drawing.Size(23, 22);
            this.bTramificar.Text = "toolStripButton9";
            this.bTramificar.ToolTipText = "Tramificar";
            this.bTramificar.Click += new System.EventHandler(this.MAbreSeleccionTramos);
            // 
            // bPremiadas
            // 
            this.bPremiadas.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bPremiadas.Image = ((System.Drawing.Image)(resources.GetObject("bPremiadas.Image")));
            this.bPremiadas.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.bPremiadas.Name = "bPremiadas";
            this.bPremiadas.Size = new System.Drawing.Size(23, 22);
            this.bPremiadas.Text = "toolStripButton10";
            this.bPremiadas.ToolTipText = "Premiadas";
            this.bPremiadas.Click += new System.EventHandler(this.MAbreSelectorPremiadas);
            // 
            // bEstimacion
            // 
            this.bEstimacion.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bEstimacion.Image = ((System.Drawing.Image)(resources.GetObject("bEstimacion.Image")));
            this.bEstimacion.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.bEstimacion.Name = "bEstimacion";
            this.bEstimacion.Size = new System.Drawing.Size(23, 22);
            this.bEstimacion.Text = "toolStripButton11";
            this.bEstimacion.ToolTipText = "Estimación de premios";
            this.bEstimacion.Click += new System.EventHandler(this.MEstimacion);
            // 
            // bBancoPruebas
            // 
            this.bBancoPruebas.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bBancoPruebas.Image = ((System.Drawing.Image)(resources.GetObject("bBancoPruebas.Image")));
            this.bBancoPruebas.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.bBancoPruebas.Name = "bBancoPruebas";
            this.bBancoPruebas.Size = new System.Drawing.Size(23, 22);
            this.bBancoPruebas.Text = "toolStripButton12";
            this.bBancoPruebas.ToolTipText = "Banco de pruebas";
            this.bBancoPruebas.Click += new System.EventHandler(this.MBancoPruebas);
            // 
            // bImportExport
            // 
            this.bImportExport.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bImportExport.Image = ((System.Drawing.Image)(resources.GetObject("bImportExport.Image")));
            this.bImportExport.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.bImportExport.Name = "bImportExport";
            this.bImportExport.Size = new System.Drawing.Size(23, 22);
            this.bImportExport.Text = "toolStripButton13";
            this.bImportExport.ToolTipText = "Importar / Exportar";
            this.bImportExport.Click += new System.EventHandler(this.MImportExport);
            // 
            // bAnalisisGrupos
            // 
            this.bAnalisisGrupos.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bAnalisisGrupos.Image = ((System.Drawing.Image)(resources.GetObject("bAnalisisGrupos.Image")));
            this.bAnalisisGrupos.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.bAnalisisGrupos.Name = "bAnalisisGrupos";
            this.bAnalisisGrupos.Size = new System.Drawing.Size(23, 22);
            this.bAnalisisGrupos.Text = "toolStripButton1";
            this.bAnalisisGrupos.ToolTipText = "Análisis de grupos";
            this.bAnalisisGrupos.Click += new System.EventHandler(this.menuItem54_Click);
            // 
            // bRedPerfectas
            // 
            this.bRedPerfectas.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bRedPerfectas.Image = ((System.Drawing.Image)(resources.GetObject("bRedPerfectas.Image")));
            this.bRedPerfectas.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.bRedPerfectas.Name = "bRedPerfectas";
            this.bRedPerfectas.Size = new System.Drawing.Size(23, 22);
            this.bRedPerfectas.Text = "toolStripButton2";
            this.bRedPerfectas.ToolTipText = "Reducciones perfectas";
            this.bRedPerfectas.Click += new System.EventHandler(this.MnuReduccionesPerfectas_Click);
            // 
            // bDependenciaLineal
            // 
            this.bDependenciaLineal.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bDependenciaLineal.Image = ((System.Drawing.Image)(resources.GetObject("bDependenciaLineal.Image")));
            this.bDependenciaLineal.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.bDependenciaLineal.Name = "bDependenciaLineal";
            this.bDependenciaLineal.Size = new System.Drawing.Size(23, 22);
            this.bDependenciaLineal.Text = "toolStripButton1";
            this.bDependenciaLineal.ToolTipText = "Dependencia lineal";
            this.bDependenciaLineal.Click += new System.EventHandler(this.MDepLineal);
            // 
            // tsCombinacion
            // 
            this.tsCombinacion.Dock = System.Windows.Forms.DockStyle.None;
            this.tsCombinacion.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.bCalcular,
            this.bCalcularM,
            this.bVerBoletos,
            this.bImprimirBoletos,
            this.bReducir,
            this.bEscrutinio,
            this.bEscrutarComb,
            this.bAnalisisColumnas,
            this.bAnalisisFallos,
            this.bAnalisisGrafico,
            this.bAnalisisSignos,
            this.bProbabilidades,
            this.bEstadisticas,
            this.bP15});
            this.tsCombinacion.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
            this.tsCombinacion.Location = new System.Drawing.Point(383, 0);
            this.tsCombinacion.Name = "tsCombinacion";
            this.tsCombinacion.Size = new System.Drawing.Size(311, 25);
            this.tsCombinacion.TabIndex = 4;
            this.tsCombinacion.Visible = false;
            // 
            // bCalcular
            // 
            this.bCalcular.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bCalcular.Image = ((System.Drawing.Image)(resources.GetObject("bCalcular.Image")));
            this.bCalcular.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.bCalcular.Name = "bCalcular";
            this.bCalcular.Size = new System.Drawing.Size(23, 22);
            this.bCalcular.Text = "toolStripButton1";
            this.bCalcular.ToolTipText = "Calcular combinación";
            this.bCalcular.Click += new System.EventHandler(this.MCalcular);
            // 
            // bCalcularM
            // 
            this.bCalcularM.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bCalcularM.Image = ((System.Drawing.Image)(resources.GetObject("bCalcularM.Image")));
            this.bCalcularM.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.bCalcularM.Name = "bCalcularM";
            this.bCalcularM.Size = new System.Drawing.Size(23, 22);
            this.bCalcularM.Text = "toolStripButton2";
            this.bCalcularM.ToolTipText = "Calcular múltiples combinaciones";
            this.bCalcularM.Click += new System.EventHandler(this.MCalcularMult);
            // 
            // bVerBoletos
            // 
            this.bVerBoletos.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bVerBoletos.Image = ((System.Drawing.Image)(resources.GetObject("bVerBoletos.Image")));
            this.bVerBoletos.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.bVerBoletos.Name = "bVerBoletos";
            this.bVerBoletos.Size = new System.Drawing.Size(23, 22);
            this.bVerBoletos.Text = "toolStripButton3";
            this.bVerBoletos.ToolTipText = "Ver boletos";
            this.bVerBoletos.Click += new System.EventHandler(this.MAbreVisualizadorBoletos);
            // 
            // bImprimirBoletos
            // 
            this.bImprimirBoletos.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bImprimirBoletos.Image = ((System.Drawing.Image)(resources.GetObject("bImprimirBoletos.Image")));
            this.bImprimirBoletos.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.bImprimirBoletos.Name = "bImprimirBoletos";
            this.bImprimirBoletos.Size = new System.Drawing.Size(23, 22);
            this.bImprimirBoletos.Text = "toolStripButton4";
            this.bImprimirBoletos.ToolTipText = "Imprimir boletos";
            this.bImprimirBoletos.Click += new System.EventHandler(this.MAbreImprimirBoletos);
            // 
            // bReducir
            // 
            this.bReducir.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bReducir.Image = ((System.Drawing.Image)(resources.GetObject("bReducir.Image")));
            this.bReducir.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.bReducir.Name = "bReducir";
            this.bReducir.Size = new System.Drawing.Size(23, 22);
            this.bReducir.Text = "toolStripButton5";
            this.bReducir.ToolTipText = "Reducir";
            this.bReducir.Click += new System.EventHandler(this.MReducir);
            // 
            // bEscrutinio
            // 
            this.bEscrutinio.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bEscrutinio.Image = ((System.Drawing.Image)(resources.GetObject("bEscrutinio.Image")));
            this.bEscrutinio.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.bEscrutinio.Name = "bEscrutinio";
            this.bEscrutinio.Size = new System.Drawing.Size(23, 22);
            this.bEscrutinio.Text = "toolStripButton6";
            this.bEscrutinio.ToolTipText = "Escrutinio";
            this.bEscrutinio.Click += new System.EventHandler(this.MEscrutinio);
            // 
            // bEscrutarComb
            // 
            this.bEscrutarComb.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bEscrutarComb.Enabled = false;
            this.bEscrutarComb.Image = ((System.Drawing.Image)(resources.GetObject("bEscrutarComb.Image")));
            this.bEscrutarComb.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.bEscrutarComb.Name = "bEscrutarComb";
            this.bEscrutarComb.Size = new System.Drawing.Size(23, 22);
            this.bEscrutarComb.Text = "Escrutar combinaciones";
            this.bEscrutarComb.ToolTipText = "Escrutar combinaciones";
            this.bEscrutarComb.Click += new System.EventHandler(this.bEscrutarComb_Click);
            // 
            // bAnalisisColumnas
            // 
            this.bAnalisisColumnas.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bAnalisisColumnas.Image = ((System.Drawing.Image)(resources.GetObject("bAnalisisColumnas.Image")));
            this.bAnalisisColumnas.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.bAnalisisColumnas.Name = "bAnalisisColumnas";
            this.bAnalisisColumnas.Size = new System.Drawing.Size(23, 22);
            this.bAnalisisColumnas.Text = "toolStripButton7";
            this.bAnalisisColumnas.ToolTipText = "Análisis de columnas";
            this.bAnalisisColumnas.Click += new System.EventHandler(this.mAnalizarColumnas);
            // 
            // bAnalisisFallos
            // 
            this.bAnalisisFallos.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bAnalisisFallos.Image = ((System.Drawing.Image)(resources.GetObject("bAnalisisFallos.Image")));
            this.bAnalisisFallos.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.bAnalisisFallos.Name = "bAnalisisFallos";
            this.bAnalisisFallos.Size = new System.Drawing.Size(23, 22);
            this.bAnalisisFallos.Text = "toolStripButton8";
            this.bAnalisisFallos.ToolTipText = "Análisis de fallos";
            this.bAnalisisFallos.Click += new System.EventHandler(this.mAnalizaCombinacion);
            // 
            // bAnalisisGrafico
            // 
            this.bAnalisisGrafico.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bAnalisisGrafico.Image = ((System.Drawing.Image)(resources.GetObject("bAnalisisGrafico.Image")));
            this.bAnalisisGrafico.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.bAnalisisGrafico.Name = "bAnalisisGrafico";
            this.bAnalisisGrafico.Size = new System.Drawing.Size(23, 22);
            this.bAnalisisGrafico.Text = "toolStripButton9";
            this.bAnalisisGrafico.ToolTipText = "Análisis gráfico";
            this.bAnalisisGrafico.Visible = false;
            this.bAnalisisGrafico.Click += new System.EventHandler(this.MAbreGraficoCombinacion);
            // 
            // bAnalisisSignos
            // 
            this.bAnalisisSignos.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bAnalisisSignos.Image = ((System.Drawing.Image)(resources.GetObject("bAnalisisSignos.Image")));
            this.bAnalisisSignos.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.bAnalisisSignos.Name = "bAnalisisSignos";
            this.bAnalisisSignos.Size = new System.Drawing.Size(23, 22);
            this.bAnalisisSignos.Text = "toolStripButton10";
            this.bAnalisisSignos.ToolTipText = "Análisis de signos";
            this.bAnalisisSignos.Click += new System.EventHandler(this.MAnalisisSignos);
            // 
            // bProbabilidades
            // 
            this.bProbabilidades.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bProbabilidades.Image = ((System.Drawing.Image)(resources.GetObject("bProbabilidades.Image")));
            this.bProbabilidades.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.bProbabilidades.Name = "bProbabilidades";
            this.bProbabilidades.Size = new System.Drawing.Size(23, 22);
            this.bProbabilidades.Text = "toolStripButton11";
            this.bProbabilidades.ToolTipText = "Probabilidades";
            this.bProbabilidades.Click += new System.EventHandler(this.MProbabilidades);
            // 
            // bEstadisticas
            // 
            this.bEstadisticas.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bEstadisticas.Image = ((System.Drawing.Image)(resources.GetObject("bEstadisticas.Image")));
            this.bEstadisticas.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.bEstadisticas.Name = "bEstadisticas";
            this.bEstadisticas.Size = new System.Drawing.Size(23, 22);
            this.bEstadisticas.Text = "toolStripButton12";
            this.bEstadisticas.ToolTipText = "Estadísticas";
            this.bEstadisticas.Click += new System.EventHandler(this.MEstadisticas);
            // 
            // bP15
            // 
            this.bP15.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bP15.Image = ((System.Drawing.Image)(resources.GetObject("bP15.Image")));
            this.bP15.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.bP15.Name = "bP15";
            this.bP15.Size = new System.Drawing.Size(23, 22);
            this.bP15.Text = "toolStripButton1";
            this.bP15.ToolTipText = "Añadir pleno al 15";
            this.bP15.Click += new System.EventHandler(this.MAbrePonerP15);
            // 
            // tsFiltros
            // 
            this.tsFiltros.Dock = System.Windows.Forms.DockStyle.None;
            this.tsFiltros.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.bCombinarFiltros,
            this.bDiferenciasFiltros,
            this.bFiltroCoincidencias,
            this.bFiltroAidomnou,
            this.bFiltroPim});
            this.tsFiltros.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
            this.tsFiltros.Location = new System.Drawing.Point(446, 0);
            this.tsFiltros.Name = "tsFiltros";
            this.tsFiltros.Size = new System.Drawing.Size(127, 25);
            this.tsFiltros.TabIndex = 6;
            this.tsFiltros.Visible = false;
            // 
            // bCombinarFiltros
            // 
            this.bCombinarFiltros.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bCombinarFiltros.Image = ((System.Drawing.Image)(resources.GetObject("bCombinarFiltros.Image")));
            this.bCombinarFiltros.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.bCombinarFiltros.Name = "bCombinarFiltros";
            this.bCombinarFiltros.Size = new System.Drawing.Size(23, 22);
            this.bCombinarFiltros.ToolTipText = "Combinar filtros";
            this.bCombinarFiltros.Click += new System.EventHandler(this.MAbreFiltros);
            // 
            // bDiferenciasFiltros
            // 
            this.bDiferenciasFiltros.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bDiferenciasFiltros.Image = ((System.Drawing.Image)(resources.GetObject("bDiferenciasFiltros.Image")));
            this.bDiferenciasFiltros.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.bDiferenciasFiltros.Name = "bDiferenciasFiltros";
            this.bDiferenciasFiltros.Size = new System.Drawing.Size(23, 22);
            this.bDiferenciasFiltros.Text = "toolStripButton2";
            this.bDiferenciasFiltros.ToolTipText = "Diferencias de filtros";
            this.bDiferenciasFiltros.Click += new System.EventHandler(this.MDifEntreFiltros);
            // 
            // bFiltroCoincidencias
            // 
            this.bFiltroCoincidencias.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bFiltroCoincidencias.Image = ((System.Drawing.Image)(resources.GetObject("bFiltroCoincidencias.Image")));
            this.bFiltroCoincidencias.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.bFiltroCoincidencias.Name = "bFiltroCoincidencias";
            this.bFiltroCoincidencias.Size = new System.Drawing.Size(23, 22);
            this.bFiltroCoincidencias.Text = "toolStripButton3";
            this.bFiltroCoincidencias.ToolTipText = "Filtro de coincidencias";
            this.bFiltroCoincidencias.Click += new System.EventHandler(this.MAbreFiltroCoincidencias);
            // 
            // bFiltroAidomnou
            // 
            this.bFiltroAidomnou.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bFiltroAidomnou.Image = ((System.Drawing.Image)(resources.GetObject("bFiltroAidomnou.Image")));
            this.bFiltroAidomnou.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.bFiltroAidomnou.Name = "bFiltroAidomnou";
            this.bFiltroAidomnou.Size = new System.Drawing.Size(23, 22);
            this.bFiltroAidomnou.Text = "toolStripButton4";
            this.bFiltroAidomnou.ToolTipText = "Filtro Aidomnou";
            this.bFiltroAidomnou.Click += new System.EventHandler(this.mFiltroAidomnou_Click);
            // 
            // bFiltroPim
            // 
            this.bFiltroPim.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bFiltroPim.Image = ((System.Drawing.Image)(resources.GetObject("bFiltroPim.Image")));
            this.bFiltroPim.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.bFiltroPim.Name = "bFiltroPim";
            this.bFiltroPim.Size = new System.Drawing.Size(23, 22);
            this.bFiltroPim.Text = "toolStripButton5";
            this.bFiltroPim.ToolTipText = "Filtro Pim";
            this.bFiltroPim.Click += new System.EventHandler(this.mFiltroPim_Click);
            // 
            // menuItem5
            // 
            this.menuItem5.Name = "menuItem5";
            this.menuItem5.Size = new System.Drawing.Size(32, 19);
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(72, 25);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(230, 16);
            this.label2.TabIndex = 1;
            this.label2.Text = "(Selecciona)";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // toolStripContainer3
            // 
            // 
            // toolStripContainer3.ContentPanel
            // 
            this.toolStripContainer3.ContentPanel.Size = new System.Drawing.Size(771, 1);
            this.toolStripContainer3.Location = new System.Drawing.Point(0, 0);
            this.toolStripContainer3.Name = "toolStripContainer3";
            this.toolStripContainer3.Size = new System.Drawing.Size(771, 25);
            this.toolStripContainer3.TabIndex = 21;
            this.toolStripContainer3.Text = "toolStripContainer3";
            // 
            // toolStripContainer3.TopToolStripPanel
            // 
            this.toolStripContainer3.TopToolStripPanel.Controls.Add(this.mainMenu);
            // 
            // toolStripContainer4
            // 
            // 
            // toolStripContainer4.ContentPanel
            // 
            this.toolStripContainer4.ContentPanel.Size = new System.Drawing.Size(771, 25);
            this.toolStripContainer4.Location = new System.Drawing.Point(0, 25);
            this.toolStripContainer4.Name = "toolStripContainer4";
            this.toolStripContainer4.Size = new System.Drawing.Size(771, 25);
            this.toolStripContainer4.TabIndex = 22;
            this.toolStripContainer4.Text = "toolStripContainer4";
            // 
            // toolStripContainer4.TopToolStripPanel
            // 
            this.toolStripContainer4.TopToolStripPanel.Controls.Add(this.tsOperaciones);
            this.toolStripContainer4.TopToolStripPanel.Controls.Add(this.tsFree);
            this.toolStripContainer4.TopToolStripPanel.Controls.Add(this.tsArchivo);
            this.toolStripContainer4.TopToolStripPanel.Controls.Add(this.tsUtilidades);
            this.toolStripContainer4.TopToolStripPanel.Controls.Add(this.tsCombinacion);
            this.toolStripContainer4.TopToolStripPanel.Controls.Add(this.tsFiltros);
            // 
            // toolStripButton3
            // 
            this.toolStripButton3.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton3.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton3.Image")));
            this.toolStripButton3.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton3.Name = "toolStripButton3";
            this.toolStripButton3.Size = new System.Drawing.Size(23, 22);
            this.toolStripButton3.Text = "toolStripButton3";
            // 
            // toolStripButton4
            // 
            this.toolStripButton4.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton4.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton4.Image")));
            this.toolStripButton4.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton4.Name = "toolStripButton4";
            this.toolStripButton4.Size = new System.Drawing.Size(23, 22);
            this.toolStripButton4.Text = "toolStripButton4";
            // 
            // toolStripButton5
            // 
            this.toolStripButton5.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton5.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton5.Image")));
            this.toolStripButton5.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton5.Name = "toolStripButton5";
            this.toolStripButton5.Size = new System.Drawing.Size(23, 22);
            this.toolStripButton5.Text = "toolStripButton5";
            this.toolStripButton5.ToolTipText = "Filtro Pim";
            // 
            // imgListNotificaciones
            // 
            this.imgListNotificaciones.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imgListNotificaciones.ImageStream")));
            this.imgListNotificaciones.TransparentColor = System.Drawing.Color.Transparent;
            this.imgListNotificaciones.Images.SetKeyName(0, "email.gif");
            this.imgListNotificaciones.Images.SetKeyName(1, "email_open.gif");
            // 
            // chkFiltroColsParcial
            // 
            this.chkFiltroColsParcial.Alineacion = Free1X2.alignment.Horizontal;
            this.chkFiltroColsParcial.BackColor = System.Drawing.Color.Bisque;
            this.chkFiltroColsParcial.Enabled = false;
            this.chkFiltroColsParcial.Estado = Free1X2.UI.Controls.CtrSemaforo.NombreEstado.Neutro;
            this.chkFiltroColsParcial.ForeColor = System.Drawing.Color.Black;
            this.chkFiltroColsParcial.Location = new System.Drawing.Point(8, 26);
            this.chkFiltroColsParcial.Name = "chkFiltroColsParcial";
            this.chkFiltroColsParcial.NumLuces = Free1X2.UI.Controls.CtrSemaforo.Luces.Dos;
            this.chkFiltroColsParcial.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.chkFiltroColsParcial.Size = new System.Drawing.Size(32, 20);
            this.chkFiltroColsParcial.TabIndex = 3;
            this.chkFiltroColsParcial.TabStop = false;
            // 
            // chkFiltroCols
            // 
            this.chkFiltroCols.Alineacion = Free1X2.alignment.Horizontal;
            this.chkFiltroCols.BackColor = System.Drawing.Color.Bisque;
            this.chkFiltroCols.Enabled = false;
            this.chkFiltroCols.Estado = Free1X2.UI.Controls.CtrSemaforo.NombreEstado.Neutro;
            this.chkFiltroCols.ForeColor = System.Drawing.Color.Black;
            this.chkFiltroCols.Location = new System.Drawing.Point(8, 26);
            this.chkFiltroCols.Name = "chkFiltroCols";
            this.chkFiltroCols.NumLuces = Free1X2.UI.Controls.CtrSemaforo.Luces.Dos;
            this.chkFiltroCols.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.chkFiltroCols.Size = new System.Drawing.Size(32, 20);
            this.chkFiltroCols.TabIndex = 3;
            this.chkFiltroCols.TabStop = false;
            // 
            // pronosticos
            // 
            this.pronosticos.BackColor = System.Drawing.Color.Bisque;
            this.pronosticos.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pronosticos.ForeColor = System.Drawing.Color.Black;
            this.pronosticos.GrupoPantalla = 0;
            this.pronosticos.GuardarGrupo = true;
            this.pronosticos.ListaPronosticos = new string[] {
        null,
        null,
        null,
        null,
        null,
        null,
        null,
        null,
        null,
        null,
        null,
        null,
        null,
        null,
        null,
        null};
            this.pronosticos.Location = new System.Drawing.Point(13, 24);
            this.pronosticos.Name = "pronosticos";
            this.pronosticos.NombreGrupo = "";
            this.pronosticos.NumPartidos = 16;
            this.pronosticos.Size = new System.Drawing.Size(344, 382);
            this.pronosticos.TabIndex = 4;
            // 
            // chkDiferencias
            // 
            this.chkDiferencias.Alineacion = Free1X2.alignment.Vertical;
            this.chkDiferencias.BackColor = System.Drawing.Color.Bisque;
            this.chkDiferencias.Estado = Free1X2.UI.Controls.CtrSemaforo.NombreEstado.Neutro;
            this.chkDiferencias.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkDiferencias.ForeColor = System.Drawing.Color.Black;
            this.chkDiferencias.Location = new System.Drawing.Point(167, 216);
            this.chkDiferencias.Name = "chkDiferencias";
            this.chkDiferencias.NumLuces = Free1X2.UI.Controls.CtrSemaforo.Luces.Dos;
            this.chkDiferencias.Size = new System.Drawing.Size(15, 28);
            this.chkDiferencias.TabIndex = 40;
            this.chkDiferencias.BotonPulsado += new System.EventHandler(this.chkSimetriasII_BotonPulsado);
            // 
            // checkSimetrias
            // 
            this.checkSimetrias.Alineacion = Free1X2.alignment.Vertical;
            this.checkSimetrias.BackColor = System.Drawing.Color.Bisque;
            this.checkSimetrias.Estado = Free1X2.UI.Controls.CtrSemaforo.NombreEstado.Neutro;
            this.checkSimetrias.ForeColor = System.Drawing.Color.Black;
            this.checkSimetrias.Location = new System.Drawing.Point(6, 216);
            this.checkSimetrias.Name = "checkSimetrias";
            this.checkSimetrias.NumLuces = Free1X2.UI.Controls.CtrSemaforo.Luces.Dos;
            this.checkSimetrias.Size = new System.Drawing.Size(15, 28);
            this.checkSimetrias.TabIndex = 38;
            this.checkSimetrias.BotonPulsado += new System.EventHandler(this.chkSimetrias_BotonPulsado);
            // 
            // chkFormatos123
            // 
            this.chkFormatos123.Alineacion = Free1X2.alignment.Vertical;
            this.chkFormatos123.BackColor = System.Drawing.Color.Bisque;
            this.chkFormatos123.Estado = Free1X2.UI.Controls.CtrSemaforo.NombreEstado.Neutro;
            this.chkFormatos123.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkFormatos123.ForeColor = System.Drawing.Color.Black;
            this.chkFormatos123.Location = new System.Drawing.Point(167, 184);
            this.chkFormatos123.Name = "chkFormatos123";
            this.chkFormatos123.NumLuces = Free1X2.UI.Controls.CtrSemaforo.Luces.Dos;
            this.chkFormatos123.Size = new System.Drawing.Size(15, 28);
            this.chkFormatos123.TabIndex = 36;
            this.chkFormatos123.BotonPulsado += new System.EventHandler(this.chkFormatos123_BotonPulsado);
            // 
            // checkIfThen
            // 
            this.checkIfThen.Alineacion = Free1X2.alignment.Vertical;
            this.checkIfThen.BackColor = System.Drawing.Color.Bisque;
            this.checkIfThen.Estado = Free1X2.UI.Controls.CtrSemaforo.NombreEstado.Neutro;
            this.checkIfThen.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkIfThen.ForeColor = System.Drawing.Color.Maroon;
            this.checkIfThen.Location = new System.Drawing.Point(167, 266);
            this.checkIfThen.Name = "checkIfThen";
            this.checkIfThen.NumLuces = Free1X2.UI.Controls.CtrSemaforo.Luces.Dos;
            this.checkIfThen.Size = new System.Drawing.Size(15, 32);
            this.checkIfThen.TabIndex = 34;
            this.checkIfThen.BotonPulsado += new System.EventHandler(this.checkIfThen_BotonPulsado);
            // 
            // checkFormatos
            // 
            this.checkFormatos.Alineacion = Free1X2.alignment.Vertical;
            this.checkFormatos.BackColor = System.Drawing.Color.Bisque;
            this.checkFormatos.Estado = Free1X2.UI.Controls.CtrSemaforo.NombreEstado.Neutro;
            this.checkFormatos.ForeColor = System.Drawing.Color.Black;
            this.checkFormatos.Location = new System.Drawing.Point(6, 184);
            this.checkFormatos.Name = "checkFormatos";
            this.checkFormatos.NumLuces = Free1X2.UI.Controls.CtrSemaforo.Luces.Dos;
            this.checkFormatos.Size = new System.Drawing.Size(15, 28);
            this.checkFormatos.TabIndex = 32;
            this.checkFormatos.BotonPulsado += new System.EventHandler(this.checkFormatos_BotonPulsado);
            // 
            // checkContactos
            // 
            this.checkContactos.Alineacion = Free1X2.alignment.Vertical;
            this.checkContactos.BackColor = System.Drawing.Color.Bisque;
            this.checkContactos.Estado = Free1X2.UI.Controls.CtrSemaforo.NombreEstado.Neutro;
            this.checkContactos.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkContactos.ForeColor = System.Drawing.Color.Black;
            this.checkContactos.Location = new System.Drawing.Point(167, 152);
            this.checkContactos.Name = "checkContactos";
            this.checkContactos.NumLuces = Free1X2.UI.Controls.CtrSemaforo.Luces.Dos;
            this.checkContactos.Size = new System.Drawing.Size(15, 28);
            this.checkContactos.TabIndex = 31;
            this.checkContactos.BotonPulsado += new System.EventHandler(this.checkContactos_BotonPulsado);
            // 
            // checkGruposEquipos
            // 
            this.checkGruposEquipos.Alineacion = Free1X2.alignment.Vertical;
            this.checkGruposEquipos.BackColor = System.Drawing.Color.Bisque;
            this.checkGruposEquipos.Estado = Free1X2.UI.Controls.CtrSemaforo.NombreEstado.Neutro;
            this.checkGruposEquipos.ForeColor = System.Drawing.Color.Black;
            this.checkGruposEquipos.Location = new System.Drawing.Point(6, 152);
            this.checkGruposEquipos.Name = "checkGruposEquipos";
            this.checkGruposEquipos.NumLuces = Free1X2.UI.Controls.CtrSemaforo.Luces.Dos;
            this.checkGruposEquipos.Size = new System.Drawing.Size(15, 28);
            this.checkGruposEquipos.TabIndex = 30;
            this.checkGruposEquipos.BotonPulsado += new System.EventHandler(this.checkGruposEquipos_BotonPulsado);
            // 
            // checkDistancias
            // 
            this.checkDistancias.Alineacion = Free1X2.alignment.Vertical;
            this.checkDistancias.BackColor = System.Drawing.Color.Bisque;
            this.checkDistancias.Estado = Free1X2.UI.Controls.CtrSemaforo.NombreEstado.Neutro;
            this.checkDistancias.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkDistancias.ForeColor = System.Drawing.Color.Black;
            this.checkDistancias.Location = new System.Drawing.Point(167, 120);
            this.checkDistancias.Name = "checkDistancias";
            this.checkDistancias.NumLuces = Free1X2.UI.Controls.CtrSemaforo.Luces.Dos;
            this.checkDistancias.Size = new System.Drawing.Size(15, 28);
            this.checkDistancias.TabIndex = 29;
            this.checkDistancias.BotonPulsado += new System.EventHandler(this.checkDistancias_BotonPulsado);
            // 
            // checkInterrupciones
            // 
            this.checkInterrupciones.Alineacion = Free1X2.alignment.Vertical;
            this.checkInterrupciones.BackColor = System.Drawing.Color.Bisque;
            this.checkInterrupciones.Estado = Free1X2.UI.Controls.CtrSemaforo.NombreEstado.Neutro;
            this.checkInterrupciones.ForeColor = System.Drawing.Color.Black;
            this.checkInterrupciones.Location = new System.Drawing.Point(6, 120);
            this.checkInterrupciones.Name = "checkInterrupciones";
            this.checkInterrupciones.NumLuces = Free1X2.UI.Controls.CtrSemaforo.Luces.Dos;
            this.checkInterrupciones.Size = new System.Drawing.Size(15, 28);
            this.checkInterrupciones.TabIndex = 28;
            this.checkInterrupciones.BotonPulsado += new System.EventHandler(this.checkInterrupciones_BotonPulsado);
            // 
            // checkCP
            // 
            this.checkCP.Alineacion = Free1X2.alignment.Vertical;
            this.checkCP.BackColor = System.Drawing.Color.Bisque;
            this.checkCP.Estado = Free1X2.UI.Controls.CtrSemaforo.NombreEstado.Neutro;
            this.checkCP.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkCP.ForeColor = System.Drawing.Color.Black;
            this.checkCP.Location = new System.Drawing.Point(167, 88);
            this.checkCP.Name = "checkCP";
            this.checkCP.NumLuces = Free1X2.UI.Controls.CtrSemaforo.Luces.Dos;
            this.checkCP.Size = new System.Drawing.Size(15, 28);
            this.checkCP.TabIndex = 27;
            this.checkCP.BotonPulsado += new System.EventHandler(this.checkCP_BotonPulsado);
            // 
            // checkDibujos
            // 
            this.checkDibujos.Alineacion = Free1X2.alignment.Vertical;
            this.checkDibujos.BackColor = System.Drawing.Color.Bisque;
            this.checkDibujos.Estado = Free1X2.UI.Controls.CtrSemaforo.NombreEstado.Neutro;
            this.checkDibujos.ForeColor = System.Drawing.Color.Black;
            this.checkDibujos.Location = new System.Drawing.Point(6, 88);
            this.checkDibujos.Name = "checkDibujos";
            this.checkDibujos.NumLuces = Free1X2.UI.Controls.CtrSemaforo.Luces.Dos;
            this.checkDibujos.Size = new System.Drawing.Size(15, 28);
            this.checkDibujos.TabIndex = 26;
            this.checkDibujos.BotonPulsado += new System.EventHandler(this.checkDibujos_BotonPulsado);
            // 
            // checkValoracion
            // 
            this.checkValoracion.Alineacion = Free1X2.alignment.Vertical;
            this.checkValoracion.BackColor = System.Drawing.Color.Bisque;
            this.checkValoracion.Estado = Free1X2.UI.Controls.CtrSemaforo.NombreEstado.Neutro;
            this.checkValoracion.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkValoracion.ForeColor = System.Drawing.Color.Black;
            this.checkValoracion.Location = new System.Drawing.Point(167, 56);
            this.checkValoracion.Name = "checkValoracion";
            this.checkValoracion.NumLuces = Free1X2.UI.Controls.CtrSemaforo.Luces.Dos;
            this.checkValoracion.Size = new System.Drawing.Size(15, 28);
            this.checkValoracion.TabIndex = 25;
            this.checkValoracion.BotonPulsado += new System.EventHandler(this.checkValoracion_BotonPulsado);
            // 
            // checkSigSeguidos
            // 
            this.checkSigSeguidos.Alineacion = Free1X2.alignment.Vertical;
            this.checkSigSeguidos.BackColor = System.Drawing.Color.Bisque;
            this.checkSigSeguidos.Estado = Free1X2.UI.Controls.CtrSemaforo.NombreEstado.Neutro;
            this.checkSigSeguidos.ForeColor = System.Drawing.Color.Black;
            this.checkSigSeguidos.Location = new System.Drawing.Point(6, 56);
            this.checkSigSeguidos.Name = "checkSigSeguidos";
            this.checkSigSeguidos.NumLuces = Free1X2.UI.Controls.CtrSemaforo.Luces.Dos;
            this.checkSigSeguidos.Size = new System.Drawing.Size(15, 28);
            this.checkSigSeguidos.TabIndex = 24;
            this.checkSigSeguidos.BotonPulsado += new System.EventHandler(this.checkSigSeguidos_BotonPulsado);
            // 
            // checkPesosNum
            // 
            this.checkPesosNum.Alineacion = Free1X2.alignment.Vertical;
            this.checkPesosNum.BackColor = System.Drawing.Color.Bisque;
            this.checkPesosNum.Estado = Free1X2.UI.Controls.CtrSemaforo.NombreEstado.Neutro;
            this.checkPesosNum.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkPesosNum.ForeColor = System.Drawing.Color.Black;
            this.checkPesosNum.Location = new System.Drawing.Point(167, 24);
            this.checkPesosNum.Name = "checkPesosNum";
            this.checkPesosNum.NumLuces = Free1X2.UI.Controls.CtrSemaforo.Luces.Dos;
            this.checkPesosNum.Size = new System.Drawing.Size(15, 28);
            this.checkPesosNum.TabIndex = 23;
            this.checkPesosNum.BotonPulsado += new System.EventHandler(this.checkPesosNum_BotonPulsado);
            // 
            // checkNoVariantes
            // 
            this.checkNoVariantes.Alineacion = Free1X2.alignment.Vertical;
            this.checkNoVariantes.BackColor = System.Drawing.Color.Bisque;
            this.checkNoVariantes.Estado = Free1X2.UI.Controls.CtrSemaforo.NombreEstado.Neutro;
            this.checkNoVariantes.ForeColor = System.Drawing.Color.Black;
            this.checkNoVariantes.Location = new System.Drawing.Point(6, 24);
            this.checkNoVariantes.Name = "checkNoVariantes";
            this.checkNoVariantes.NumLuces = Free1X2.UI.Controls.CtrSemaforo.Luces.Dos;
            this.checkNoVariantes.Size = new System.Drawing.Size(15, 28);
            this.checkNoVariantes.TabIndex = 22;
            this.checkNoVariantes.BotonPulsado += new System.EventHandler(this.checkNoVariantes_BotonPulsado);
            // 
            // ctrSemaforo1
            // 
            this.ctrSemaforo1.Alineacion = Free1X2.alignment.Vertical;
            this.ctrSemaforo1.BackColor = System.Drawing.Color.Bisque;
            this.ctrSemaforo1.Estado = Free1X2.UI.Controls.CtrSemaforo.NombreEstado.Neutro;
            this.ctrSemaforo1.Location = new System.Drawing.Point(166, 222);
            this.ctrSemaforo1.Name = "ctrSemaforo1";
            this.ctrSemaforo1.NumLuces = Free1X2.UI.Controls.CtrSemaforo.Luces.Dos;
            this.ctrSemaforo1.Size = new System.Drawing.Size(18, 32);
            this.ctrSemaforo1.TabIndex = 36;
            // 
            // pctNotificaciones
            // 
            this.pctNotificaciones.Location = new System.Drawing.Point(41, 515);
            this.pctNotificaciones.Name = "pctNotificaciones";
            this.pctNotificaciones.Size = new System.Drawing.Size(16, 16);
            this.pctNotificaciones.TabIndex = 10;
            this.pctNotificaciones.TabStop = false;
            this.toolTip1.SetToolTip(this.pctNotificaciones, "No Hay Notificaciones Nuevas");
            this.pctNotificaciones.Click += new System.EventHandler(this.pctNotificaciones_Click);
            // 
            // MainForm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(6, 14);
            this.BackColor = System.Drawing.Color.Bisque;
            this.ClientSize = new System.Drawing.Size(779, 616);
            this.Controls.Add(this.toolStripContainer4);
            this.Controls.Add(this.toolStripContainer3);
            this.Controls.Add(this.toolStripContainer1);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MainMenuStrip = this.mainMenu;
            this.MaximumSize = new System.Drawing.Size(787, 650);
            this.MinimumSize = new System.Drawing.Size(787, 650);
            this.Name = "MainForm";
            this.Padding = new System.Windows.Forms.Padding(0, 0, 0, 10);
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Free1X2";
            this.Load += new System.EventHandler(this.MainFormLoad);
            this.Activated += new System.EventHandler(this.MainFormActivated);
            this.Closing += new System.ComponentModel.CancelEventHandler(this.MainForm_Closing);
            this.groupBox.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.gbFiltroGeneral.ResumeLayout(false);
            this.gbFiltroGeneral.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.imgCerrar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.imgAbrir)).EndInit();
            this.toolStripContainer1.ContentPanel.ResumeLayout(false);
            this.toolStripContainer1.ResumeLayout(false);
            this.toolStripContainer1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pBQuinielista)).EndInit();
            this.gbFiltroParcial.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.mainMenu.ResumeLayout(false);
            this.mainMenu.PerformLayout();
            this.tsFree.ResumeLayout(false);
            this.tsFree.PerformLayout();
            this.tsArchivo.ResumeLayout(false);
            this.tsArchivo.PerformLayout();
            this.tsOperaciones.ResumeLayout(false);
            this.tsOperaciones.PerformLayout();
            this.tsUtilidades.ResumeLayout(false);
            this.tsUtilidades.PerformLayout();
            this.tsCombinacion.ResumeLayout(false);
            this.tsCombinacion.PerformLayout();
            this.tsFiltros.ResumeLayout(false);
            this.tsFiltros.PerformLayout();
            this.toolStripContainer3.TopToolStripPanel.ResumeLayout(false);
            this.toolStripContainer3.TopToolStripPanel.PerformLayout();
            this.toolStripContainer3.ResumeLayout(false);
            this.toolStripContainer3.PerformLayout();
            this.toolStripContainer4.TopToolStripPanel.ResumeLayout(false);
            this.toolStripContainer4.TopToolStripPanel.PerformLayout();
            this.toolStripContainer4.ResumeLayout(false);
            this.toolStripContainer4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pctNotificaciones)).EndInit();
            this.ResumeLayout(false);

		}
		#endregion

		
	}
}
