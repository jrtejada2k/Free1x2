using System;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Free1X2.MotorCalculo;

namespace Free1X2.UI
{
    public partial class ListadoCondicionesFrm : Form
    {
        string[] equipos;
        string[] pronosticos;
        string archivoFiltro;
        GrupoPartidos grupoDePartidos;
        ControladorGrupos controladorDegrupos;
        ControladorIfThen controladorDeIfThen;

        public ListadoCondicionesFrm()
        {
            InitializeComponent();
            FormulariosHelper fH = new FormulariosHelper();
            fH.Traducir(this);
        }
        public string[] Equipos
        {
            get { return equipos; }
            set { equipos = value; }
        }
        public string[] Pronosticos
        {
            get { return pronosticos; }
            set { pronosticos = value; }
        }
        public string ArchivoFiltro
        {
            get { return archivoFiltro; }
            set { archivoFiltro = value; }
        }
        public GrupoPartidos GrupoDePartidos
        {
            get { return grupoDePartidos;}
            set { grupoDePartidos = value; }
        }
        public ControladorGrupos ControladorDeGrupos
        {
            get { return controladorDegrupos; }
            set { controladorDegrupos = value; }
        }
        public ControladorIfThen ControladorDeIfThen
        {
            get { return controladorDeIfThen; }
            set { controladorDeIfThen = value; }
        }
        protected string ObtenerStringPronosticoCP(ColumnaProbable columna)
        {
            string signosCP = "";
            for (int l = 0; l < columna.Pronosticos.Length; l++)
            {
                if (columna.Pronosticos[l] != "")
                {
                    signosCP += columna.Pronosticos[l];
                }
                else
                {
                    signosCP += "*";
                }
                if (l < columna.Pronosticos.Length - 1)
                {
                    signosCP += ",";
                }
            } return signosCP;
        }

        private void ListadoCondicionesFrm_Load(object sender, EventArgs e)
        {
            TreeNode nodoPrincipal = new TreeNode("Listado de Condiciones",0,0);
            
            treeVwCondiciones.Nodes.Add(nodoPrincipal);
            #region Pronostico Base
            TreeNode nodoPronostico = new TreeNode("Pronóstico Base", 1, 1);
            nodoPrincipal.Nodes.Add(nodoPronostico);
            //Añadir Pronósticos
            for (int i = 0; i < Pronosticos.Length; i++)
            {
                string partido = Convert.ToString(i + 1);
                TreeNode nodoPronosticos = new TreeNode("Partido " + partido + ": " + pronosticos[i],1,1);
                nodoPronostico.Nodes.Add(nodoPronosticos);
            } 
            #endregion
            #region Filtro de Columnas
            //Especificar si se usa filtro
            TreeNode nodoFiltro = new TreeNode("Filtro de Columnas",2,2);
            if (ArchivoFiltro == "")
            {
                TreeNode nodoNombreFiltro = new TreeNode("No se usa ningún Filtro",6,6);
                nodoFiltro.Nodes.Add(nodoNombreFiltro);
            }
            else
            {
                TreeNode nodoNombreFiltro = new TreeNode(ArchivoFiltro,6,6);
                nodoFiltro.Nodes.Add(nodoNombreFiltro);
            }
            nodoPrincipal.Nodes.Add(nodoFiltro); 
            #endregion
            #region Grupos
            //Grupos
            TreeNode nodoDeGrupos = new TreeNode("Grupos",3,3);
            for (int i = 0; i < GrupoDePartidos.CtrlGrupos.GruposPartidos.Count; i++)
            {
                string nombreGrupo = "Grupo " + Convert.ToString(i);
                if (i == 0)
                {
                    //Es el boleto base
                    nombreGrupo = "Boleto Base";
                }
                Grupo grupo = grupoDePartidos.CtrlGrupos.GruposPartidos[i];
                //Condiciones

                TreeNode nodoGrupo = new TreeNode(nombreGrupo,3,3);

                TreeNode nodoPartidosActivosGrupo = new TreeNode("Partidos Activos: " + grupo.ObtenPartidosActivos(),7,7);
                nodoGrupo.Nodes.Add(nodoPartidosActivosGrupo);
                if (grupo.UsaFiltroParcial)
                {
                    TreeNode nodoFiltroParcial = new TreeNode("Filtro Parcial: " + grupo.ArchivoFiltroGrupo, 2, 2);
                    nodoGrupo.Nodes.Add(nodoFiltroParcial);
                }
                TreeNode nodoCondiciones = new TreeNode("Condiciones",8,8);
                nodoGrupo.Nodes.Add(nodoCondiciones);
                for (int j = 0; j < grupo.Filtros.Count; j++)
                {
                    IFiltro filtro = grupo.Filtros[j];
                    if (filtro.IsActive)
                    {
                        TreeNode nodoFiltroCondiciones = new TreeNode(filtro.NombreFiltro.ToString());
                        switch (filtro.NombreFiltro.ToString())
                        {
                            #region VX2
                            case "NoVariantes":
                                nodoFiltroCondiciones.ImageIndex = 9;
                                FiltroNoVariantes filtroVariantes = (FiltroNoVariantes)grupo.GetFiltro(Filtro.NoVariantes.ToString());
                                TreeNode nodoV = new TreeNode("Cantidad de V: " + filtroVariantes.GetVariantes(),9,9);
                                TreeNode nodoX = new TreeNode("Cantidad de X: " + filtroVariantes.GetEquis(),9,9);
                                TreeNode nodo2 = new TreeNode("Cantidad de 2: " + filtroVariantes.GetDoses(),9,9);
                                nodoFiltroCondiciones.Nodes.Add(nodoV);
                                nodoFiltroCondiciones.Nodes.Add(nodoX);
                                nodoFiltroCondiciones.Nodes.Add(nodo2);
                                break;
                            #endregion
                            #region SSeguidos
                            case "SignosSeguidos":
                                nodoFiltroCondiciones.ImageIndex = 10;
                                FiltroSignosSeguidos filtroSeguidos = (FiltroSignosSeguidos)grupo.GetFiltro(Filtro.SignosSeguidos.ToString());
                                TreeNode nodoVSeg = new TreeNode("Cantidad de V Seguidas: " + filtroSeguidos.GetVariantes(),10,10);
                                TreeNode nodo1Seg = new TreeNode("Cantidad de 1 Seguidos: " + filtroSeguidos.GetUnos(),10,10);
                                TreeNode nodoXSeg = new TreeNode("Cantidad de X Seguidas: " + filtroSeguidos.GetEquis(),10,10);
                                TreeNode nodo2Seg = new TreeNode("Cantidad de 2 Seguidos: " + filtroSeguidos.GetDoses(),10,10);
                                nodoFiltroCondiciones.Nodes.Add(nodoVSeg);
                                nodoFiltroCondiciones.Nodes.Add(nodo1Seg);
                                nodoFiltroCondiciones.Nodes.Add(nodoXSeg);
                                nodoFiltroCondiciones.Nodes.Add(nodo2Seg);
                                break;
                            #endregion
                            #region Dibujos
                            case "Dibujos":
                                nodoFiltroCondiciones.ImageIndex = 11;
                                FiltroDibujos filtroDibujos = (FiltroDibujos)grupo.GetFiltro(Filtro.Dibujos.ToString());
                                TreeNode nodoDibujos = new TreeNode("Dibujos : " + filtroDibujos.GetDibujos(),11,11);
                                nodoFiltroCondiciones.Nodes.Add(nodoDibujos);
                                break;
                            #endregion
                            #region CProbables
                            case "ColProbables":
                                nodoFiltroCondiciones.ImageIndex = 12;
                                FiltroColProbables filtroCP = (FiltroColProbables)grupo.GetFiltro(Filtro.ColProbables.ToString());
                                TreeNode nodoCP = new TreeNode("Columnas Probables");
                                nodoFiltroCondiciones.Nodes.Add(nodoCP);
                                for (int k = 0; k < filtroCP.ColProbables.Count; k++)
                                {
                                    
                                    ColumnaProbable columna = (ColumnaProbable)filtroCP.ColProbables[k];
                                    string signosCP = ObtenerStringPronosticoCP(columna);

                                    TreeNode nodoCPs = new TreeNode("Columna Probable: " + signosCP,12,12);
                                    TreeNode nodoAciertos = new TreeNode("Aciertos: " + columna.GetAciertos(), 12, 12);
                                    TreeNode nodoAciertosSeg = new TreeNode("Aciertos Seguidos: " + columna.GetAciertosSeguidos(), 12, 12);
                                    TreeNode nodoFallosSeg = new TreeNode("Fallos Seguidos: " + columna.GetFallosSeguidos(), 12, 12);
                                    TreeNode nodoToleraciasAciertos = new TreeNode("Tolerancias Aciertos: " + columna.GetACTol(), 12, 12);
                                    TreeNode nodoToleraciasAciertosSeg = new TreeNode("Tolerancias Aciertos Seguidos: " + columna.GetACSTol(), 12, 12);
                                    TreeNode nodoToleraciasFallosSeg = new TreeNode("Tolerancias Fallos Seguidos: " + columna.GetFSTol(), 12, 12);
                                    TreeNode nodoAciertosTol = new TreeNode("Aciertos en Tolerancias: " + columna.GetTolerancias(), 12, 12);
                                    TreeNode nodoPuntos = new TreeNode("Puntos: " + columna.GetPuntos(), 12, 12);

                                    //Añadir nodos
                                    if (nodoAciertos.Text != "Aciertos: ") nodoCPs.Nodes.Add(nodoAciertos);
                                    if (nodoAciertosSeg.Text != "Aciertos Seguidos: ") nodoCPs.Nodes.Add(nodoAciertosSeg);
                                    if (nodoFallosSeg.Text != "Fallos Seguidos: ") nodoCPs.Nodes.Add(nodoFallosSeg);
                                    if (nodoToleraciasAciertos.Text != "Tolerancias Aciertos: ") nodoCPs.Nodes.Add(nodoToleraciasAciertos);
                                    if (nodoToleraciasAciertosSeg.Text != "Tolerancias Aciertos Seguidos: ") nodoCPs.Nodes.Add(nodoToleraciasAciertosSeg);
                                    if (nodoToleraciasFallosSeg.Text != "Tolerancias Fallos Seguidos: ") nodoCPs.Nodes.Add(nodoToleraciasFallosSeg);
                                    if (nodoAciertosTol.Text != "Aciertos en Tolerancias: ") nodoCPs.Nodes.Add(nodoAciertosTol);
                                    if (nodoPuntos.Text != "Puntos: ") nodoCPs.Nodes.Add(nodoPuntos);


                                    nodoCP.Nodes.Add(nodoCPs);

                                }
                                //Relaciones
                                TreeNode nodoRelacionesCP = new TreeNode("Relaciones", 12, 12);
                                for (int a = 0; a < filtroCP.RelacionesCP1.Relaciones.Count; a++)
                                {
                                    RelacionCP1 relCP = (RelacionCP1)filtroCP.RelacionesCP1.Relaciones[a];
                                    TreeNode nodoRelacionCP = new TreeNode("Columnas: " + relCP.Columnas, 12, 12);
                                    TreeNode nodoRelacionCPCuantas = new TreeNode("Cuantas: " + relCP.CantidadCP, 12, 12);
                                    TreeNode nodoRelacionCPAciertos = new TreeNode("Aciertos: " + relCP.CantidadCP, 12, 12);

                                    TreeNode nodoRelacionCPSumaAciertos = new TreeNode("Suma Aciertos: " + relCP.SumaAciertos, 12, 12);
                                    TreeNode nodoRelacionCPRecorrido = new TreeNode("Recorrido: " + relCP.Recorridos, 12, 12);

                                    //nodoRelacionCP.Nodes.Add(nodoRelacionCP);
                                    nodoRelacionCP.Nodes.Add(nodoRelacionCPCuantas);
                                    nodoRelacionCP.Nodes.Add(nodoRelacionCPAciertos);
                                    nodoRelacionCP.Nodes.Add(nodoRelacionCPSumaAciertos);
                                    nodoRelacionCP.Nodes.Add(nodoRelacionCPRecorrido);


                                    nodoRelacionesCP.Nodes.Add(nodoRelacionCP);
                                }
                                nodoFiltroCondiciones.Nodes.Add(nodoRelacionesCP);


                                //Relaciones2
                                TreeNode nodoRelacionesCP2 = new TreeNode("Relaciones II", 12, 12);
                                for (int a = 0; a < filtroCP.RelacionesCP2.Relaciones2.Count; a++)
                                {
                                    RelacionCP2 relCP2 = filtroCP.RelacionesCP2.Relaciones2[a];
                                    TreeNode nodoRelII = new TreeNode("Relación II", 12, 12);
                                    TreeNode nodoRelacionCP2A = new TreeNode("Las columnas: " + relCP2.StrColsA + " tendrán " + relCP2.StrAciertos + " " + relCP2.Concepto + " " + relCP2.Cantidad + " que las columnas " + relCP2.StrColsB, 12, 12);
                                    TreeNode nodoRelacionCP2B = new TreeNode("Las columnas: " + relCP2.StrColsA2 + " tendrán " + relCP2.StrAciertos2 + " " + relCP2.Concepto2 + " " + relCP2.Cantidad2 + " que las columnas " + relCP2.StrColsB2, 12, 12);

                                    nodoRelII.Nodes.Add(nodoRelacionCP2A);
                                    nodoRelII.Nodes.Add(nodoRelacionCP2B);

                                    nodoRelacionesCP2.Nodes.Add(nodoRelII);
                                }
                                nodoFiltroCondiciones.Nodes.Add(nodoRelacionesCP2);

                                //Relaciones3
                                TreeNode nodoRelacionesCP3 = new TreeNode("Relaciones III", 12, 12);
                                for (int a = 0; a < filtroCP.RelacionesCP3.Relaciones.Count; a++)
                                {
                                    RelacionCP3 relCP3 = filtroCP.RelacionesCP3.Relaciones[a];
                                    TreeNode nodoRelIII = new TreeNode("Relación III", 12, 12);
                                    TreeNode nodoRelacionCP3Cols = new TreeNode("Columnas: " + relCP3.ColumnasImplicadasString, 12, 12);
                                    TreeNode nodoRelacionCP3Concepto = new TreeNode("Concepto: " + relCP3.ConceptoString, 12, 12);
                                    TreeNode nodoRelacionCP3Sandwinchs = new TreeNode("Sandwichs: " + relCP3.NumeroSandwichsPermitidosString, 12, 12);
                                    TreeNode nodoRelacionCP3Escaleras = new TreeNode("Escaleras", 12, 12);
                                         TreeNode nodoRelacionCP3EscalerasTot = new TreeNode("Totales: " + relCP3.NumeroEscalerasTotalesPermitidasString, 12, 12);
                                         TreeNode nodoRelacionCP3EscalerasAsc = new TreeNode("Ascendentes: " + relCP3.NumeroEscalerasASCPermitidasString, 12, 12);
                                         TreeNode nodoRelacionCP3EscalerasDesc = new TreeNode("Descendentes: " + relCP3.NumeroEscalerasDESCPermitidasString, 12, 12);
                                    
                                    nodoRelacionCP3Escaleras.Nodes.Add(nodoRelacionCP3EscalerasTot);
                                    nodoRelacionCP3Escaleras.Nodes.Add(nodoRelacionCP3EscalerasAsc);
                                    nodoRelacionCP3Escaleras.Nodes.Add(nodoRelacionCP3EscalerasDesc);

                                    TreeNode nodoRelacionCP3AgrupacionesPasoFijo = new TreeNode("Agrupaciones Paso Fijo", 12, 12);
                                         TreeNode nodoAgrupacionPasoFijo = new TreeNode("Agrupación", 12, 12);
                                         for (int b = 0; b < relCP3.AgrupacionesPasoFijoPermitidas.Count; b++)
                                         {
                                             AgrupacionColumnas agrup = relCP3.AgrupacionesPasoFijoPermitidas[b];
                                             TreeNode nodoAgrupacionPasoFijoNum = new TreeNode("Número: " + Utils.UtilidadesEntradasValores.ObtenerTextoFromListAciertos(agrup.Numero), 12, 12);
                                             TreeNode nodoAgrupacionPasoFijoElementos = new TreeNode("Elementos: " + agrup.NoElementos, 12, 12);
                                             TreeNode nodoAgrupacionPasoFijoAciertos = new TreeNode("Aciertos: " + Utils.UtilidadesEntradasValores.ObtenerTextoFromListAciertos(agrup.NoAciertos), 12, 12);

                                             nodoAgrupacionPasoFijo.Nodes.Add(nodoAgrupacionPasoFijoNum);
                                             nodoAgrupacionPasoFijo.Nodes.Add(nodoAgrupacionPasoFijoElementos);
                                             nodoAgrupacionPasoFijo.Nodes.Add(nodoAgrupacionPasoFijoAciertos);

                                         }
                                         nodoRelacionCP3AgrupacionesPasoFijo.Nodes.Add(nodoAgrupacionPasoFijo);

                                         TreeNode nodoRelacionCP3AgrupacionesSolapadas = new TreeNode("Agrupaciones Solapadas", 12, 12);
                                         TreeNode nodoAgrupacionSol = new TreeNode("Agrupación", 12, 12);
                                         for (int b = 0; b < relCP3.AgrupacionesSolapadasPermitidas.Count; b++)
                                         {
                                             AgrupacionColumnas agrup = relCP3.AgrupacionesSolapadasPermitidas[b];
                                             TreeNode nodoAgrupacionSolNum = new TreeNode("Número: " + Utils.UtilidadesEntradasValores.ObtenerTextoFromListAciertos(agrup.Numero), 12, 12);
                                             TreeNode nodoAgrupacionSolElementos = new TreeNode("Elementos: " + agrup.NoElementos.ToString(), 12, 12);
                                             TreeNode nodoAgrupacionSolAciertos = new TreeNode("Aciertos: " + Utils.UtilidadesEntradasValores.ObtenerTextoFromListAciertos(agrup.NoAciertos), 12, 12);

                                             nodoAgrupacionSol.Nodes.Add(nodoAgrupacionSolNum);
                                             nodoAgrupacionSol.Nodes.Add(nodoAgrupacionSolElementos);
                                             nodoAgrupacionSol.Nodes.Add(nodoAgrupacionSolAciertos);

                                         }
                                         nodoRelacionCP3AgrupacionesSolapadas.Nodes.Add(nodoAgrupacionSol);

                                    nodoRelIII.Nodes.Add(nodoRelacionCP3Cols);
                                    nodoRelIII.Nodes.Add(nodoRelacionCP3Concepto);
                                    nodoRelIII.Nodes.Add(nodoRelacionCP3Sandwinchs);
                                    nodoRelIII.Nodes.Add(nodoRelacionCP3Escaleras);
                                    nodoRelIII.Nodes.Add(nodoRelacionCP3AgrupacionesPasoFijo);
                                    nodoRelIII.Nodes.Add(nodoRelacionCP3AgrupacionesSolapadas);
                                    nodoRelacionesCP3.Nodes.Add(nodoRelIII);
                                }
                                nodoFiltroCondiciones.Nodes.Add(nodoRelacionesCP3);


                                //Control de fallos
                                TreeNode nodoControlFallosCP = new TreeNode("Control de Fallos", 12, 1);
                                for (int a = 0; a < filtroCP.ControlFallosCP.ControlesFallos.Count; a++)
                                {
                                    CPControlFallos controlFallosCP = (CPControlFallos)filtroCP.ControlFallosCP.ControlesFallos[a];
                                    TreeNode nodoControlesFallosCP = new TreeNode("Control Fallos", 12, 12);
                                    TreeNode nodoControlFallosColumnas = new TreeNode("Columnas: " + controlFallosCP.Columnas, 12, 12);
                                    TreeNode nodoControlFallosAciertos = new TreeNode("Aciertos: " + controlFallosCP.Aciertos, 12, 12);
                                    TreeNode nodoControlFallosTol = new TreeNode("Tolerancia: " + controlFallosCP.Tolerancias, 12, 12);

                                    nodoControlesFallosCP.Nodes.Add(nodoControlFallosColumnas);
                                    nodoControlesFallosCP.Nodes.Add(nodoControlFallosTol);
                                    nodoControlesFallosCP.Nodes.Add(nodoControlFallosAciertos);

                                    nodoControlFallosCP.Nodes.Add(nodoControlesFallosCP);
                                }
                                TreeNode nodoNoFallosControlesCP = new TreeNode("Numero de Fallos de Controles: " + filtroCP.ControlFallosCP.FallosPermitidos, 12, 12);
                                nodoControlFallosCP.Nodes.Add(nodoNoFallosControlesCP);
                                nodoFiltroCondiciones.Nodes.Add(nodoControlFallosCP);
                                break;
                            #endregion
                            #region Interrupciones
                            case "NoInterrupciones":
                                nodoFiltroCondiciones.ImageIndex = 13;
                                FiltroInterrupciones filtroInt = (FiltroInterrupciones)grupo.GetFiltro(Filtro.NoInterrupciones.ToString());

                                TreeNode nodoInterrupciones = new TreeNode("Interrupciones", 13, 13);
                                TreeNode nodoIntGlobal = new TreeNode("Interrupciones Globales: " + filtroInt.GetIntGlobales(), 13, 13);
                                TreeNode nodoInt1 = new TreeNode("Interrupciones de 1: " + filtroInt.GetInt1(), 13, 13);
                                TreeNode nodoIntX = new TreeNode("Interrupciones de X: " + filtroInt.GetIntX(), 13, 13);
                                TreeNode nodoInt2 = new TreeNode("Interrupciones de 2 " + filtroInt.GetInt2(), 13, 13);

                                TreeNode nodoIntGlobalSeg = new TreeNode("Interrupciones Globales Seguidas: " + filtroInt.GetIntGlobalSeg(), 13, 13);
                                TreeNode nodoInt1Seg = new TreeNode("Interrupciones de 1 Seguidas: " + filtroInt.GetInt1Seg(), 13, 13);
                                TreeNode nodoIntXSeg = new TreeNode("Interrupciones de X Seguidas: " + filtroInt.GetIntXSeg(), 13, 13);
                                TreeNode nodoInt2Seg = new TreeNode("Interrupciones de 2 Seguidas: " + filtroInt.GetInt2Seg(), 13, 13);

                                if (nodoIntGlobal.Text != "") nodoFiltroCondiciones.Nodes.Add(nodoIntGlobal);
                                if (nodoInt1.Text != "") nodoFiltroCondiciones.Nodes.Add(nodoInt1);
                                if (nodoIntX.Text != "") nodoFiltroCondiciones.Nodes.Add(nodoIntX);
                                if (nodoInt2.Text != "") nodoFiltroCondiciones.Nodes.Add(nodoInt2);

                                if (nodoIntGlobalSeg.Text != "") nodoFiltroCondiciones.Nodes.Add(nodoIntGlobalSeg);
                                if (nodoInt1Seg.Text != "") nodoFiltroCondiciones.Nodes.Add(nodoInt1Seg);
                                if (nodoIntXSeg.Text != "") nodoFiltroCondiciones.Nodes.Add(nodoIntXSeg);
                                if (nodoInt2Seg.Text != "") nodoFiltroCondiciones.Nodes.Add(nodoInt2Seg);

                                break;
                            #endregion
                            #region Pesos
                            case "PesosNumericos":
                                nodoFiltroCondiciones.ImageIndex = 14;
                                FiltroPesosNumericos filtroPesos = (FiltroPesosNumericos)grupo.GetFiltro(Filtro.PesosNumericos.ToString());
                                TreeNode nodoPesosGlobales = new TreeNode("Peso Global: " + filtroPesos.GetPNGlobal(), 14, 14);
                                TreeNode nodoPesosV = new TreeNode("Peso Variantes: " + filtroPesos.GetPNVariantes(), 14, 14);
                                TreeNode nodoPesos1 = new TreeNode("Peso 1: " + filtroPesos.GetPNUnos(), 14, 14);
                                TreeNode nodoPesosX = new TreeNode("Peso X: " + filtroPesos.GetPNEquis(), 14, 14);
                                TreeNode nodoPesos2 = new TreeNode("Peso 2: " + filtroPesos.GetPNDoses(), 14, 14);

                                TreeNode nodoPesosGlobalesTol = new TreeNode("Tolerancias Peso Global: " + filtroPesos.GetPNGlobalTol(), 14, 14);
                                TreeNode nodoPesosVTol = new TreeNode("Tolerancias Peso Variantes: " + filtroPesos.GetPNVariantesTol(), 14, 14);
                                TreeNode nodoPesos1Tol = new TreeNode("Tolerancias Peso 1: " + filtroPesos.GetPNUnosTol(), 14, 14);
                                TreeNode nodoPesosXTol = new TreeNode("Tolerancias Peso X: " + filtroPesos.GetPNEquisTol(), 14, 14);
                                TreeNode nodoPesos2Tol = new TreeNode("Tolerancias Peso 2: " + filtroPesos.GetPNDosesTol(), 14, 14);

                                TreeNode nodoPesosTolerancias = new TreeNode("Número de Tolerancias: " + filtroPesos.GetTolerancias(), 14, 14);

                                nodoFiltroCondiciones.Nodes.Add(nodoPesosGlobales);
                                nodoFiltroCondiciones.Nodes.Add(nodoPesosV);
                                nodoFiltroCondiciones.Nodes.Add(nodoPesos1);
                                nodoFiltroCondiciones.Nodes.Add(nodoPesosX);
                                nodoFiltroCondiciones.Nodes.Add(nodoPesos2);
                                if (nodoPesosGlobalesTol.Text != "Tolerancias Peso Global: ") nodoFiltroCondiciones.Nodes.Add(nodoPesosGlobalesTol);
                                if (nodoPesosVTol.Text != "Tolerancias Peso Variantes: ") nodoFiltroCondiciones.Nodes.Add(nodoPesosVTol);
                                if (nodoPesos1Tol.Text != "Tolerancias Peso 1: ") nodoFiltroCondiciones.Nodes.Add(nodoPesos1Tol);
                                if (nodoPesosXTol.Text != "Tolerancias Peso X: ") nodoFiltroCondiciones.Nodes.Add(nodoPesosXTol);
                                if (nodoPesos2Tol.Text != "Tolerancias Peso 2: ") nodoFiltroCondiciones.Nodes.Add(nodoPesos2Tol);
                                if (nodoPesosTolerancias.Text != "Número de Tolerancias: ") nodoFiltroCondiciones.Nodes.Add(nodoPesosTolerancias);


                                break;
                            #endregion
                            #region valoraciones
                            case "ValoracionSignos":
                                nodoFiltroCondiciones.ImageIndex = 15;
                                FiltroValoracionSignos filtroVal = (FiltroValoracionSignos)grupo.GetFiltro(Filtro.ValoracionSignos.ToString());
                                TreeNode nodoValoracion = new TreeNode("Valoración", 15,15);
                                for (int z = 0; z < filtroVal.Valores1.Length; z++)
                                {
                                    TreeNode nodoValores = new TreeNode(filtroVal.Valores1[z].ToString() + "-" + filtroVal.ValoresX[z].ToString() + "-" + filtroVal.Valores2[z].ToString(), 15, 15);

                                    nodoValoracion.Nodes.Add(nodoValores);

                                }
                                TreeNode nodoLimitesGlobal = new TreeNode("Límites Valoración Global: " + filtroVal.ValorGlobal, 15, 15);
                                TreeNode nodoLimites1 = new TreeNode("Límites Valoración 1: " + filtroVal.ValorUnos, 15, 15);
                                TreeNode nodoLimitesX = new TreeNode("Límites Valoración X: " + filtroVal.ValorEquis, 15, 15);
                                TreeNode nodoLimites2 = new TreeNode("Límites Valoración 2: " + filtroVal.ValorDoses, 15, 15);

                                TreeNode nodoTipoVal = new TreeNode("Tipo de Valoración: " + filtroVal.TipoValoracion, 15, 15);

                                nodoFiltroCondiciones.Nodes.Add(nodoValoracion);
                                nodoFiltroCondiciones.Nodes.Add(nodoLimitesGlobal);
                                nodoFiltroCondiciones.Nodes.Add(nodoLimites1);
                                nodoFiltroCondiciones.Nodes.Add(nodoLimitesX);
                                nodoFiltroCondiciones.Nodes.Add(nodoLimites2);

                                nodoFiltroCondiciones.Nodes.Add(nodoTipoVal);


                                break;
                            #endregion
                            #region Distancias
                            case "Distancias":
                                nodoFiltroCondiciones.ImageIndex = 16;
                                FiltroDistancias filtroD = (FiltroDistancias)grupo.GetFiltro(Filtro.Distancias.ToString());
                                TreeNode nodoDistV = new TreeNode("Distancias de Variantes: " + filtroD.GetIntVar(), 16, 16);
                                TreeNode nodoDist1 = new TreeNode("Distancias de 1: " + filtroD.GetInt1(), 16, 16);
                                TreeNode nodoDistX = new TreeNode("Distancias de X: " + filtroD.GetIntX(), 16, 16);
                                TreeNode nodoDist2 = new TreeNode("Distancias de 2: " + filtroD.GetInt2(), 16, 16);

                                nodoFiltroCondiciones.Nodes.Add(nodoDistV);
                                nodoFiltroCondiciones.Nodes.Add(nodoDist1);
                                nodoFiltroCondiciones.Nodes.Add(nodoDistX);
                                nodoFiltroCondiciones.Nodes.Add(nodoDist2);

                                break;
                            #endregion
                            #region GruposEquipos
                            case "GruposEquipos":
                                nodoFiltroCondiciones.ImageIndex = 21;
                                FiltroGruposEquipos filtroGE = (FiltroGruposEquipos)grupo.GetFiltro(Filtro.GruposEquipos.ToString());
                                TreeNode nodoRelacionesGE = new TreeNode("Relaciones", 21, 1);
                                for (int z = 0; z < filtroGE.GruposEquipos.Count; z++)
                                {
                                    TreeNode nodoConjuntoGE = new TreeNode("Conjunto " + Convert.ToString(z + 1), 21,21);
                                    TreeNode nodoEquiposMarcados = new TreeNode("Equipos Marcados", 21, 21);
                                    nodoConjuntoGE.Nodes.Add(nodoEquiposMarcados);
                                    GrupoEquipos grupoE = (GrupoEquipos)filtroGE.GruposEquipos[z];
                                    for (int a = 0; a < grupoE.Pronosticos.Length; a++)
                                    {
                                        char marcados = grupoE.Pronosticos[a];
                                        TreeNode nodoPartidoGE;
                                        switch (marcados)
                                        {
                                            case '1':
                                                //Marcado el local
                                                nodoPartidoGE = new TreeNode("Marcado - Desmarcado", 21, 21);
                                                nodoEquiposMarcados.Nodes.Add(nodoPartidoGE);
                                                break;
                                            case '2':
                                                //Visitante
                                                nodoPartidoGE = new TreeNode("Desmarcado - Marcado", 21, 21);
                                                nodoEquiposMarcados.Nodes.Add(nodoPartidoGE);
                                                break;
                                            case '3':
                                                //Ambos
                                                nodoPartidoGE = new TreeNode("Marcado - Marcado", 21, 21);
                                                nodoEquiposMarcados.Nodes.Add(nodoPartidoGE);
                                                break;
                                            case '0':
                                                //Ninguno
                                                nodoPartidoGE = new TreeNode("Desmarcado - Desmarcado", 21, 21);
                                                nodoEquiposMarcados.Nodes.Add(nodoPartidoGE);
                                                break;
                                        }


                                    }
                                    TreeNode nodoVictoriasGE = new TreeNode("Victorias: " + grupoE.Victorias, 21, 21);
                                    TreeNode nodoEmpatesGE = new TreeNode("Empates: " + grupoE.Empates, 21, 21);
                                    TreeNode nodoDerrotasGE = new TreeNode("Derrotas: " + grupoE.Derrotas, 21, 21);
                                    TreeNode nodoSumaPuntos = new TreeNode("Suma Puntos: " + grupoE.SumaPuntos, 21, 21);

                                    nodoConjuntoGE.Nodes.Add(nodoVictoriasGE);
                                    nodoConjuntoGE.Nodes.Add(nodoEmpatesGE);
                                    nodoConjuntoGE.Nodes.Add(nodoDerrotasGE);
                                    nodoConjuntoGE.Nodes.Add(nodoSumaPuntos);


                                    nodoFiltroCondiciones.Nodes.Add(nodoConjuntoGE);

                                }
                                for (int a = 0; a < filtroGE.RelacionesGE1.Relaciones.Count; a++)
                                {
                                    RelacionGE1 relGE = (RelacionGE1)filtroGE.RelacionesGE1.Relaciones[a];
                                    TreeNode nodoRelacionGE = new TreeNode("Grupos de Equipos: " + relGE.GruposEquipos, 21, 21);
                                    TreeNode nodoVictoriasRelGE = new TreeNode("Suma Victorias: " + relGE.SumaVictorias, 21, 21);
                                    TreeNode nodoEmpatesRelGE = new TreeNode("Suma Empates: " + relGE.SumaEmpates,21, 21);
                                    TreeNode nodoDerrotasRelGE = new TreeNode("Suma Derrotas: " + relGE.SumaDerrotas, 21, 21);
                                    TreeNode nodoSumaPuntosRelGE = new TreeNode("Suma Puntos: " + relGE.SumaPuntos, 21, 21);

                                    nodoRelacionGE.Nodes.Add(nodoVictoriasRelGE);
                                    nodoRelacionGE.Nodes.Add(nodoEmpatesRelGE);
                                    nodoRelacionGE.Nodes.Add(nodoDerrotasRelGE);
                                    nodoRelacionGE.Nodes.Add(nodoSumaPuntosRelGE);

                                    nodoRelacionesGE.Nodes.Add(nodoRelacionGE);

                                } nodoFiltroCondiciones.Nodes.Add(nodoRelacionesGE);

                                break;
                            #endregion
                            #region Contactos
                            case "Contactos":
                                nodoFiltroCondiciones.ImageIndex = 17;
                                FiltroContactos filtroCont = (FiltroContactos)grupo.GetFiltro(Filtro.Contactos.ToString());
                                TreeNode nodoC1X = new TreeNode("Contactos 1X: " + filtroCont.GetNum1X(), 17, 17);
                                TreeNode nodoC12 = new TreeNode("Contactos 12: " + filtroCont.GetNum12(), 17, 17);
                                TreeNode nodoCX2 = new TreeNode("Contactos X2: " + filtroCont.GetNumX2(), 17, 17);
                                TreeNode nodoC11 = new TreeNode("Contactos 11: " + filtroCont.GetNum11(), 17, 17);
                                TreeNode nodoCXX = new TreeNode("Contactos XX: " + filtroCont.GetNumXX(), 17, 17);
                                TreeNode nodoC22 = new TreeNode("Contactos 22: " + filtroCont.GetNum22(), 17, 17);
                                TreeNode nodoC1V = new TreeNode("Contactos 1V: " + filtroCont.GetNum1V(), 17, 17);
                                TreeNode nodoCXV = new TreeNode("Contactos XV: " + filtroCont.GetNumXV(), 17, 17);
                                TreeNode nodoC2V = new TreeNode("Contactos 2V: " + filtroCont.GetNum2V(), 17, 17);
                                TreeNode nodoCVV = new TreeNode("Contactos VV: " + filtroCont.GetNumVV(), 17, 17);

                                nodoFiltroCondiciones.Nodes.Add(nodoC1X);
                                nodoFiltroCondiciones.Nodes.Add(nodoC12);
                                nodoFiltroCondiciones.Nodes.Add(nodoCX2);
                                nodoFiltroCondiciones.Nodes.Add(nodoC11);
                                nodoFiltroCondiciones.Nodes.Add(nodoCXX);
                                nodoFiltroCondiciones.Nodes.Add(nodoC22);
                                nodoFiltroCondiciones.Nodes.Add(nodoC1V);
                                nodoFiltroCondiciones.Nodes.Add(nodoCXV);
                                nodoFiltroCondiciones.Nodes.Add(nodoC2V);
                                nodoFiltroCondiciones.Nodes.Add(nodoCVV);

                                break;
                            #endregion
                            #region FormatosSignos
                            case "FormatosSignos":
                                nodoFiltroCondiciones.ImageIndex = 18;
                                FiltroFormatosSignos filtroFormatos = (FiltroFormatosSignos)grupo.GetFiltro(Filtro.FormatosSignos.ToString());
                                TreeNode nodoFormatoSignos = new TreeNode("Conjuntos", 18, 18);
                                for (int a = 0; a < filtroFormatos.FormatosSignos.Count; a++)
                                {
                                    //Conjuntos
                                    FormatosSignos formatosSignos = (FormatosSignos)filtroFormatos.FormatosSignos[a];
                                    TreeNode nodoConjuntoFormatos = new TreeNode("Conjunto", 18, 18);
                                    for (int z = 0; z < formatosSignos.LineasFormatos.Count; z++)
                                    {
                                        FormatoSignos formatoSignos = (FormatoSignos)formatosSignos.LineasFormatos[z];
                                        TreeNode nodoFormatoSignosFormato = new TreeNode("Formato: " + formatoSignos.Formato, 18, 18);
                                        TreeNode nodoFormatoSignosRango = new TreeNode("Rango: " + formatoSignos.RangoAparicion, 18, 18);
                                        nodoFormatoSignosFormato.Nodes.Add(nodoFormatoSignosRango);

                                        nodoConjuntoFormatos.Nodes.Add(nodoFormatoSignosFormato);


                                        nodoFormatoSignos.Nodes.Add(nodoConjuntoFormatos);
                                    }
                                    TreeNode nodoFormatosLineas = new TreeNode("Lineas: " + formatosSignos.Lineas, 18, 18);
                                    TreeNode nodoFormatosGlobal = new TreeNode("Global: " + formatosSignos.Lineas, 18, 18);
                                    nodoConjuntoFormatos.Nodes.Add(nodoFormatosLineas);
                                    nodoConjuntoFormatos.Nodes.Add(nodoFormatosGlobal);
                                }
                                nodoFiltroCondiciones.Nodes.Add(nodoFormatoSignos);
                                break;
                            #endregion
                            #region Formatos123
                            case "Formatos123":
                                nodoFiltroCondiciones.ImageIndex = 19;
                                FiltroFormatos123 filtrof123 = (FiltroFormatos123)grupo.GetFiltro(Filtro.Formatos123.ToString());
                                TreeNode nodoFormatos123 = new TreeNode("Formatos", 19, 19);
                                TreeNode nodoValoracion123 = new TreeNode("Valoración", 19, 19);
                                for (int z = 0; z < filtrof123.Valores.Length / 3; z++)
                                {
                                    TreeNode nodoValores123 = new TreeNode(filtrof123.Valores[z, 0].ToString() + "-" + filtrof123.Valores[z, 1].ToString() + "-" + filtrof123.Valores[z, 2].ToString(), 19, 19);

                                    nodoValoracion123.Nodes.Add(nodoValores123);

                                }
                                nodoFiltroCondiciones.Nodes.Add(nodoValoracion123);
                                for (int z = 0; z < filtrof123.ArrayFormatos.Count; z++)
                                {
                                    Formato123 formato123 = filtrof123.ArrayFormatos[z];
                                    TreeNode nodoFormato123 = new TreeNode("Formato 123: " + formato123.Formato, 19, 19);
                                    if (!filtrof123.PasoFijo)
                                    {
                                        TreeNode nodoLimitesFormato123 = new TreeNode("Límites: " + formato123.AciertosMin.ToString() + "-" + formato123.AciertosMax.ToString(), 19, 19);

                                        nodoFormato123.Nodes.Add(nodoLimitesFormato123);
                                    }
                                    nodoFormatos123.Nodes.Add(nodoFormato123);
                                }
                                if (filtrof123.PasoFijo)
                                {
                                    TreeNode nodoF123Modo = new TreeNode("Modo: No Admitiendo Repeticiones", 19, 19);
                                    TreeNode nodoF123Aciertos = new TreeNode("Aciertos Globales: " + filtrof123.AciertosFiltro, 19, 19);
                                    nodoFormatos123.Nodes.Add(nodoF123Aciertos);
                                    nodoFormatos123.Nodes.Add(nodoF123Modo);
                                }
                                else
                                {
                                    TreeNode nodoF123Modo = new TreeNode("Modo: Admitiendo Repeticiones", 19, 19);
                                    TreeNode nodoF123Aciertos = new TreeNode("Líneas Acertadas: " + filtrof123.AciertosFiltro, 19, 19);
                                    nodoFormatos123.Nodes.Add(nodoF123Aciertos);
                                    nodoFormatos123.Nodes.Add(nodoF123Modo);

                                }


                                nodoFiltroCondiciones.Nodes.Add(nodoFormatos123);

                                break;
                            #endregion
                            #region Simetrías
                            case "Simetrias":
                                nodoFiltroCondiciones.ImageIndex = 20;
                                FiltroSimetrias filtroSim = (FiltroSimetrias)grupo.GetFiltro(Filtro.Simetrias.ToString());
                                TreeNode nodoSimetrias = new TreeNode("Simetrías",20,20);
                                for (int z = 0; z < filtroSim.ArraySimetrias.Count; z++)
                                {
                                    Simetria sim = filtroSim.ArraySimetrias[z];
                                    TreeNode nodoSimetria = new TreeNode("Simetría: " + sim.Partidos, 20, 20);
                                    nodoSimetrias.Nodes.Add(nodoSimetria);
                                }
                                TreeNode nodoAciertosSim = new TreeNode("Aciertos: " + filtroSim.Aciertos, 20, 20);

                                nodoFiltroCondiciones.Nodes.Add(nodoSimetrias);
                                nodoFiltroCondiciones.Nodes.Add(nodoAciertosSim);
                                break;
                            #endregion
                            #region Diferencias
                            case "Diferencias":
                                nodoFiltroCondiciones.ImageIndex = 23;
                                FiltroDiferencias filtroDif = (FiltroDiferencias)grupo.GetFiltro(Filtro.Diferencias.ToString());
                               // TreeNode nodoDiferencias = new TreeNode("Diferencias", 23, 23);
                                for (int z = 0; z < filtroDif.Diferencias.Count; z++)
                                {
                                    TreeNode nodoDiferencia = new TreeNode("Diferencia " + (z+1), 23, 23);

                                    Diferencia dif = filtroDif.Diferencias[z];
                                    for (int gruposPartidos = 0; gruposPartidos < dif.PartidosSimetricos.Count; gruposPartidos++)
                                    {
                                        TreeNode nodoPartidosSimetricos = new TreeNode("Partidos: " + Utils.UtilidadesEntradasValores.ObtenerTextoFromBool(dif.PartidosSimetricos[gruposPartidos]), 1, 1);
                                        nodoDiferencia.Nodes.Add(nodoPartidosSimetricos);
                                    }

                                    //Diferencias
                                    TreeNode nodoDiferenciasValores = new TreeNode("Diferencias", 23, 23);

                                    TreeNode nodoDiferenciasVariantes = new TreeNode("Variantes: " + Utils.UtilidadesEntradasValores.ObtenerTextoFromBool(dif.AcV), 9, 9);
                                    TreeNode nodoDiferenciasEquis = new TreeNode("Equis: " + Utils.UtilidadesEntradasValores.ObtenerTextoFromBool(dif.AcX), 9, 9);
                                    TreeNode nodoDiferenciasDoses = new TreeNode("Doses: " + Utils.UtilidadesEntradasValores.ObtenerTextoFromBool(dif.AcDoses), 9, 9);
                                    TreeNode nodoDiferenciasDibujos = new TreeNode("Dibujos: " + Utils.UtilidadesEntradasValores.ObtenerTextoFromBool(dif.AcDib), 11, 11);
                                    TreeNode nodoDiferenciasInterrupciones = new TreeNode("Interrupciones: " + Utils.UtilidadesEntradasValores.ObtenerTextoFromBool(dif.AcInt), 13, 13);
                                    TreeNode nodoDiferenciasFormatos = new TreeNode("Formatos: " + Utils.UtilidadesEntradasValores.ObtenerTextoFromBool(dif.AcFormatos), 18, 18);

                                    nodoDiferenciasValores.Nodes.Add(nodoDiferenciasVariantes);
                                    nodoDiferenciasValores.Nodes.Add(nodoDiferenciasEquis);
                                    nodoDiferenciasValores.Nodes.Add(nodoDiferenciasDoses);
                                    nodoDiferenciasValores.Nodes.Add(nodoDiferenciasDibujos);
                                    nodoDiferenciasValores.Nodes.Add(nodoDiferenciasInterrupciones);
                                    nodoDiferenciasValores.Nodes.Add(nodoDiferenciasFormatos);

                                    nodoDiferencia.Nodes.Add(nodoDiferenciasValores);

                                    nodoFiltroCondiciones.Nodes.Add(nodoDiferencia);
                                }
                                break;
                            #endregion
                        }
                        nodoCondiciones.Nodes.Add(nodoFiltroCondiciones);
                    }

                }
                nodoDeGrupos.Nodes.Add(nodoGrupo);
            }
            nodoPrincipal.Nodes.Add(nodoDeGrupos); 
            #endregion
            #region Control de grupos
            //Control de grupos
            TreeNode nodoControlGrupos = new TreeNode("Control de grupos",4,4);

            for (int i = 1; i < this.ControladorDeGrupos.ControlesGrupos.Count; i++)
            {
                ControlGrupos controlGrupos = this.ControladorDeGrupos.ControlesGrupos[i];
                TreeNode nodoControlDeGrupos = new TreeNode("Control de Grupos", 4, 4);
                TreeNode nodoControlGrupoGruposControlados = new TreeNode("Grupos: " + controlGrupos.ObtenGruposControlados(), 4, 4);
                TreeNode nodoControlGrupoFallosPermitidos = new TreeNode("Fallos: " + controlGrupos.ObtenFallosPermitidos(), 4, 4);
                nodoControlDeGrupos.Nodes.Add(nodoControlGrupoGruposControlados);
                nodoControlDeGrupos.Nodes.Add(nodoControlGrupoFallosPermitidos);
                nodoControlGrupos.Nodes.Add(nodoControlDeGrupos);
            }

            for (int i = 0; i < this.ControladorDeGrupos.ControlesConjuntos.Count; i++)
            {
                ControlConjuntos controlConjuntos = this.ControladorDeGrupos.ControlesConjuntos[i];

                TreeNode nodoControlDeConjuntos = new TreeNode("Control Conjuntos", 4, 4);
                TreeNode nodoControlGruposConjuntos = new TreeNode("Conjuntos: " + controlConjuntos.ObtenCtrlGruposControladosStr(), 4, 4);
                TreeNode nodoControlGrupoFallosConjuntos = new TreeNode("Fallos: " + controlConjuntos.ObtenFallosPermitidosStr(), 4, 4);
                nodoControlDeConjuntos.Nodes.Add(nodoControlGruposConjuntos);
                nodoControlDeConjuntos.Nodes.Add(nodoControlGrupoFallosConjuntos);

                nodoControlGrupos.Nodes.Add(nodoControlDeConjuntos);
            }

            nodoPrincipal.Nodes.Add(nodoControlGrupos); 
            #endregion
            #region IfThen
            //Controlador IfThen
            TreeNode nodoIfThen = new TreeNode("IfThen", 5, 5);
            if (this.ControladorDeIfThen != null)
            {
                if (this.ControladorDeIfThen.EsActivo)
                {
                    TreeNode nodoControladorIfThenCondiciones = new TreeNode("Condiciones Relacionadas",5,5);
                    for (int i = 0; i < this.ControladorDeIfThen.ControlesCondiciones.Count; i++)
                    {
                        CondicionIfThen condicionIfThen = (CondicionIfThen)this.ControladorDeIfThen.ControlesCondiciones[i];
                        TreeNode nodoControladorIfThenCondicion = new TreeNode("Condición", 5, 5);
                        TreeNode nodoControladorifThenCondicionIf = new TreeNode("Si se da: " + condicionIfThen.CondIf, 5, 5);
                        TreeNode nodoControladorifThenCondicionThen = new TreeNode("Entonces: " + condicionIfThen.CondThen, 5, 5);

                        nodoControladorIfThenCondicion.Nodes.Add(nodoControladorifThenCondicionIf);
                        nodoControladorIfThenCondicion.Nodes.Add(nodoControladorifThenCondicionThen);

                        nodoControladorIfThenCondiciones.Nodes.Add(nodoControladorIfThenCondicion);
                    }
                    TreeNode nodoIfThenAciertos = new TreeNode("Condiciones que se cumplen: " + ControladorDeIfThen.RangoAciertoCondiciones, 5, 5);
                    nodoControladorIfThenCondiciones.Nodes.Add(nodoIfThenAciertos);
                    nodoIfThen.Nodes.Add(nodoControladorIfThenCondiciones);

                    if (ControladorDeIfThen.ControlesGrupos.Count > 0)
                    {
                        TreeNode nodoControladorIfThenGrupos = new TreeNode("Grupos Relacionados", 5, 5);
                        for (int i = 0; i < ControladorDeIfThen.ControlesGrupos.Count; i++)
                        {
                            GrupoIfThen grupoIfThen = (GrupoIfThen)ControladorDeIfThen.ControlesGrupos[i];
                            TreeNode nodoControladorIfThenGrupo = new TreeNode("Grupos", 5, 5);
                            TreeNode nodoControladorifThenGrupoIf = new TreeNode("Si el grupo " + grupoIfThen.NumGrupoIf + " es " + grupoIfThen.NoIf, 5, 5);
                            TreeNode nodoControladorifThenGrupoThen = new TreeNode("Entonces el grupo: " + grupoIfThen.NumGrupoThen + " debe ser " + grupoIfThen.NoThen, 5, 5);

                            nodoControladorIfThenGrupo.Nodes.Add(nodoControladorifThenGrupoIf);
                            nodoControladorIfThenGrupo.Nodes.Add(nodoControladorifThenGrupoThen);

                            nodoControladorIfThenGrupos.Nodes.Add(nodoControladorIfThenGrupo);
                        }
                        TreeNode nodoIfThenAciertosGrupos = new TreeNode("Grupos que se cumplen: " + ControladorDeIfThen.RangoAciertoGrupos, 5, 5);
                        nodoControladorIfThenGrupos.Nodes.Add(nodoIfThenAciertosGrupos);
                        nodoIfThen.Nodes.Add(nodoControladorIfThenGrupos);
                    }



                }
            }
            else
            {
                TreeNode nodoIfThenInactivo = new TreeNode("IfThen no activado", 5, 5);
                nodoIfThen.Nodes.Add(nodoIfThenInactivo);
            } 
            

            nodoPrincipal.Nodes.Add(nodoIfThen);
            #endregion
        }
        protected string ExportarATexto(TreeNode treeND, int profundidad)
        {
            string texto = "";
            for (int i = 0; i < treeND.Nodes.Count; i++)
            {
                TreeNode nodoTemp = treeND.Nodes[i];
                if (nodoTemp.Nodes.Count > 0)
                {
                    for (int j = 0; j < profundidad; j++)
                    {
                        texto += " ";
                    }
                    texto += nodoTemp.Text;
                    texto += "\r\n";
                    texto += "\r\n";
                    
                    texto += ExportarATexto(nodoTemp, profundidad+1);
                    texto += "\r\n";
                }
                else
                {
                    for (int j = 0; j < profundidad; j++)
                    {
                        texto += " ";
                    }
                    texto += nodoTemp.Text;
                    texto += "\r\n";
                }
            }
            return texto;
        }
        protected string ExportarAHtml(TreeNode treeND, int profundidad)
        {
            string texto = "";

            for (int i = 0; i < treeND.Nodes.Count; i++)
            {
                TreeNode nodoTemp = treeND.Nodes[i];
                if (nodoTemp.Nodes.Count > 0)
                {
                    switch (profundidad)
                    {
                        case 0:
                            texto += "<br><b>";
                            break;
                        case 1:
                            texto += "<i>";
                            break;
                    }
                    texto += nodoTemp.Text;
                    switch (profundidad)
                    {
                        case 0:
                            texto += "</b><br>";
                            break;
                        case 1:
                            texto += "</i>";
                            break;
                    }
                    texto += "<br>";
                    

                    texto += ExportarAHtml(nodoTemp, profundidad + 1);
                    
                }
                else
                {
                    texto += nodoTemp.Text;
                    texto += "<br>";
                }
            }
            
            return texto;
        }

        private void btnExpandir_Click(object sender, EventArgs e)
        {
            treeVwCondiciones.ExpandAll();
        }

        private void btnColapsar_Click(object sender, EventArgs e)
        {
            treeVwCondiciones.CollapseAll();
        }

        private void Exportar_Click(object sender, EventArgs e)
        {
            string nombreArchivo = "";
            string texto = ExportarATexto(treeVwCondiciones.Nodes[0], 0);
            SaveFileDialog abreArchivo = new SaveFileDialog();
            abreArchivo.Filter = "Listados(*.txt)|*.txt";
            if (abreArchivo.ShowDialog() == DialogResult.OK)
            {
                nombreArchivo = abreArchivo.FileName;
                StreamWriter sw = new StreamWriter(nombreArchivo,false, Encoding.UTF8);
                sw.Write(texto);
                sw.Close();
            }
            if (File.Exists(nombreArchivo))
            {
                System.Diagnostics.Process.Start(nombreArchivo);
            }

        }

        private void ExportarHtml_Click(object sender, EventArgs e)
        {
            string nombreArchivo = "";
            string texto = ExportarAHtml(treeVwCondiciones.Nodes[0], 0);
            texto = "<html><head><title>Listado de Condiciones</title></head><body bgcolor=\"#DBFEBC\">" + texto;
            texto += "</body></html>";
            SaveFileDialog abreArchivo = new SaveFileDialog();
            abreArchivo.Filter = "Listados(*.html)|*.html";
            if (abreArchivo.ShowDialog() == DialogResult.OK)
            {
                nombreArchivo = abreArchivo.FileName;
                StreamWriter sw = new StreamWriter(nombreArchivo, false, Encoding.UTF8);
                sw.Write(texto);
                sw.Close();
            }
            if (File.Exists(nombreArchivo))
            {
                System.Diagnostics.Process.Start(nombreArchivo);
            }
        }



    }
}
