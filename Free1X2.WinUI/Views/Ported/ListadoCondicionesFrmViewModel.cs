// Free1X2 · WinUI 3 — WIN3
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Free1X2.MotorCalculo;
using Free1X2.Utils;
using Free1X2.WinUI.Services;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.System;

namespace Free1X2.WinUI.Views.Ported
{
    /// <summary>
    /// Nodo de presentación para el árbol de condiciones.
    /// Equivalente WinUI del System.Windows.Forms.TreeNode usado en el legacy
    /// ListadoCondicionesFrm (UI/ListadoCondicionesFrm.cs).
    /// </summary>
    public partial class CondicionNodo : ObservableObject
    {
        [ObservableProperty]
        private string _texto = string.Empty;

        [ObservableProperty]
        private bool _isExpanded;

        public ObservableCollection<CondicionNodo> Hijos { get; } = new();

        public CondicionNodo()
        {
        }

        public CondicionNodo(string texto)
        {
            _texto = texto;
        }

        /// <summary>Añade y devuelve un nodo hijo con el texto dado (azúcar de TreeNode.Nodes.Add).</summary>
        public CondicionNodo AgregarHijo(string texto)
        {
            var nodo = new CondicionNodo(texto);
            Hijos.Add(nodo);
            return nodo;
        }
    }

    /// <summary>
    /// ViewModel para ListadoCondicionesFrmPage.
    /// Port del legacy Free1X2.UI.ListadoCondicionesFrm: muestra en árbol jerárquico las
    /// condiciones/filtros configurados (Pronóstico Base, Filtro de Columnas, Grupos y sus
    /// filtros, Control de grupos, IfThen) y permite expandir/colapsar y exportar a texto/HTML.
    ///
    /// Los datos se toman del Analizador compartido (AppState.Instancia.Analizador), que en el
    /// WinForms se inyectaba por propiedades (Pronosticos, ArchivoFiltro, GrupoDePartidos,
    /// ControladorDeGrupos, ControladorDeIfThen) desde MainForm.
    /// </summary>
    public partial class ListadoCondicionesFrmViewModel : ObservableObject
    {
        // Raíz del árbol de condiciones (en el legacy: nodoPrincipal "Listado de Condiciones").
        public ObservableCollection<CondicionNodo> Condiciones { get; } = new();

        public ListadoCondicionesFrmViewModel()
        {
            Reconstruir();
        }

        /// <summary>
        /// Reconstruye el árbol como en ListadoCondicionesFrm_Load() a partir del Analizador
        /// compartido. La página la llama al navegar para reflejar el estado actual.
        /// </summary>
        public void Reconstruir()
        {
            Condiciones.Clear();

            Analizador analizador = AppState.Instancia.Analizador;
            var nodoPrincipal = new CondicionNodo("Listado de Condiciones") { IsExpanded = true };
            Condiciones.Add(nodoPrincipal);

            // ===== Pronóstico Base =====
            var nodoPronostico = nodoPrincipal.AgregarHijo("Pronóstico Base");
            string[] pronosticos = analizador.Pronosticos ?? Array.Empty<string>();
            for (int i = 0; i < pronosticos.Length; i++)
            {
                nodoPronostico.AgregarHijo("Partido " + (i + 1) + ": " + pronosticos[i]);
            }

            // ===== Filtro de Columnas =====
            var nodoFiltro = nodoPrincipal.AgregarHijo("Filtro de Columnas");
            string archivoFiltro = AppState.Instancia.ArchivoFiltroCols;
            nodoFiltro.AgregarHijo(string.IsNullOrEmpty(archivoFiltro) ? "No se usa ningún Filtro" : archivoFiltro);

            // ===== Grupos =====
            var nodoDeGrupos = nodoPrincipal.AgregarHijo("Grupos");
            GrupoPartidos gruposPartidos = analizador.GruposPartidos;
            for (int i = 0; i < gruposPartidos.Count; i++)
            {
                Grupo grupo = gruposPartidos[i];
                string nombreGrupo = i == 0 ? "Boleto Base" : "Grupo " + i;
                var nodoGrupo = nodoDeGrupos.AgregarHijo(nombreGrupo);

                nodoGrupo.AgregarHijo("Partidos Activos: " + grupo.ObtenPartidosActivos());
                if (grupo.UsaFiltroParcial)
                {
                    nodoGrupo.AgregarHijo("Filtro Parcial: " + grupo.ArchivoFiltroGrupo);
                }

                var nodoCondiciones = nodoGrupo.AgregarHijo("Condiciones");
                foreach (IFiltro filtro in grupo.Filtros)
                {
                    if (!filtro.IsActive) continue;
                    var nodoFiltroCond = nodoCondiciones.AgregarHijo(filtro.NombreFiltro.ToString());
                    // Desglose detallado por tipo de filtro, mismo árbol que el legacy
                    // Free1X2/UI/ListadoCondicionesFrm.cs (switch por NombreFiltro, líneas 143-679).
                    ConstruirDetalleFiltro(grupo, filtro, nodoFiltroCond);
                }
            }

            // ===== Control de grupos =====
            var nodoControlGrupos = nodoPrincipal.AgregarHijo("Control de grupos");
            ControladorGrupos ctrlGrupos = analizador.CtrlGrupos;
            for (int i = 1; i < ctrlGrupos.ControlesGrupos.Count; i++)
            {
                ControlGrupos cg = ctrlGrupos.ControlesGrupos[i];
                var nodoCG = nodoControlGrupos.AgregarHijo("Control de Grupos");
                nodoCG.AgregarHijo("Grupos: " + cg.ObtenGruposControlados());
                nodoCG.AgregarHijo("Fallos: " + cg.ObtenFallosPermitidos());
            }
            for (int i = 0; i < ctrlGrupos.ControlesConjuntos.Count; i++)
            {
                ControlConjuntos cConj = ctrlGrupos.ControlesConjuntos[i];
                var nodoCConj = nodoControlGrupos.AgregarHijo("Control Conjuntos");
                nodoCConj.AgregarHijo("Conjuntos: " + cConj.ObtenCtrlGruposControladosStr());
                nodoCConj.AgregarHijo("Fallos: " + cConj.ObtenFallosPermitidosStr());
            }

            // ===== IfThen =====
            var nodoIfThen = nodoPrincipal.AgregarHijo("IfThen");
            ControladorIfThen? ifThen = analizador.IfThen;
            if (ifThen != null && ifThen.EsActivo)
            {
                var nodoIfThenConds = nodoIfThen.AgregarHijo("Condiciones Relacionadas");
                foreach (CondicionIfThen cond in ifThen.ControlesCondiciones)
                {
                    var nodoCond = nodoIfThenConds.AgregarHijo("Condición");
                    nodoCond.AgregarHijo("Si se da: " + cond.CondIf);
                    nodoCond.AgregarHijo("Entonces: " + cond.CondThen);
                }
                nodoIfThenConds.AgregarHijo("Condiciones que se cumplen: " + ifThen.RangoAciertoCondiciones);

                if (ifThen.ControlesGrupos.Count > 0)
                {
                    var nodoIfThenGrupos = nodoIfThen.AgregarHijo("Grupos Relacionados");
                    foreach (GrupoIfThen gr in ifThen.ControlesGrupos)
                    {
                        var nodoGr = nodoIfThenGrupos.AgregarHijo("Grupos");
                        nodoGr.AgregarHijo("Si el grupo " + gr.NumGrupoIf + " es " + gr.NoIf);
                        nodoGr.AgregarHijo("Entonces el grupo: " + gr.NumGrupoThen + " debe ser " + gr.NoThen);
                    }
                    nodoIfThenGrupos.AgregarHijo("Grupos que se cumplen: " + ifThen.RangoAciertoGrupos);
                }
            }
            else
            {
                nodoIfThen.AgregarHijo("IfThen no activado");
            }
        }

        // ======================= Desglose detallado por filtro (port del switch legacy) =======================

        /// <summary>
        /// Construye los sub-nodos de detalle de un filtro activo, replicando 1:1 el switch por
        /// NombreFiltro de Free1X2.UI.ListadoCondicionesFrm.ListadoCondicionesFrm_Load()
        /// (Free1X2/UI/ListadoCondicionesFrm.cs, líneas 143-679). Lee de los getters del propio
        /// IFiltro vía Grupo.GetFiltro(nombre), igual que el legacy.
        /// </summary>
        private static void ConstruirDetalleFiltro(Grupo grupo, IFiltro filtro, CondicionNodo nodoFiltroCond)
        {
            switch (filtro.NombreFiltro.ToString())
            {
                // ---- VX2 (NoVariantes) — legacy líneas 146-155 ----
                case "NoVariantes":
                {
                    var f = (FiltroNoVariantes)grupo.GetFiltro(Filtro.NoVariantes.ToString());
                    nodoFiltroCond.AgregarHijo("Cantidad de V: " + f.GetVariantes());
                    nodoFiltroCond.AgregarHijo("Cantidad de X: " + f.GetEquis());
                    nodoFiltroCond.AgregarHijo("Cantidad de 2: " + f.GetDoses());
                    break;
                }

                // ---- Signos Seguidos — legacy líneas 158-169 ----
                case "SignosSeguidos":
                {
                    var f = (FiltroSignosSeguidos)grupo.GetFiltro(Filtro.SignosSeguidos.ToString());
                    nodoFiltroCond.AgregarHijo("Cantidad de V Seguidas: " + f.GetVariantes());
                    nodoFiltroCond.AgregarHijo("Cantidad de 1 Seguidos: " + f.GetUnos());
                    nodoFiltroCond.AgregarHijo("Cantidad de X Seguidas: " + f.GetEquis());
                    nodoFiltroCond.AgregarHijo("Cantidad de 2 Seguidos: " + f.GetDoses());
                    break;
                }

                // ---- Dibujos — legacy líneas 172-177 ----
                case "Dibujos":
                {
                    var f = (FiltroDibujos)grupo.GetFiltro(Filtro.Dibujos.ToString());
                    nodoFiltroCond.AgregarHijo("Dibujos : " + f.GetDibujos());
                    break;
                }

                // ---- Columnas Probables — legacy líneas 180-335 ----
                case "ColProbables":
                {
                    var f = (FiltroColProbables)grupo.GetFiltro(Filtro.ColProbables.ToString());
                    ConstruirDetalleColProbables(f, nodoFiltroCond);
                    break;
                }

                // ---- Interrupciones — legacy líneas 338-363 ----
                case "NoInterrupciones":
                {
                    var f = (FiltroInterrupciones)grupo.GetFiltro(Filtro.NoInterrupciones.ToString());
                    // El legacy crea los nodos y sólo añade los de texto no vacío. Como aquí el texto
                    // siempre lleva el prefijo (p.ej. "Interrupciones Globales: "), nunca es "" — igual
                    // que en WinForms, donde la guarda (Text != "") jamás descartaba estos nodos.
                    nodoFiltroCond.AgregarHijo("Interrupciones Globales: " + f.GetIntGlobales());
                    nodoFiltroCond.AgregarHijo("Interrupciones de 1: " + f.GetInt1());
                    nodoFiltroCond.AgregarHijo("Interrupciones de X: " + f.GetIntX());
                    nodoFiltroCond.AgregarHijo("Interrupciones de 2 " + f.GetInt2());
                    nodoFiltroCond.AgregarHijo("Interrupciones Globales Seguidas: " + f.GetIntGlobalSeg());
                    nodoFiltroCond.AgregarHijo("Interrupciones de 1 Seguidas: " + f.GetInt1Seg());
                    nodoFiltroCond.AgregarHijo("Interrupciones de X Seguidas: " + f.GetIntXSeg());
                    nodoFiltroCond.AgregarHijo("Interrupciones de 2 Seguidas: " + f.GetInt2Seg());
                    break;
                }

                // ---- Pesos Numéricos — legacy líneas 366-396 ----
                case "PesosNumericos":
                {
                    var f = (FiltroPesosNumericos)grupo.GetFiltro(Filtro.PesosNumericos.ToString());
                    nodoFiltroCond.AgregarHijo("Peso Global: " + f.GetPNGlobal());
                    nodoFiltroCond.AgregarHijo("Peso Variantes: " + f.GetPNVariantes());
                    nodoFiltroCond.AgregarHijo("Peso 1: " + f.GetPNUnos());
                    nodoFiltroCond.AgregarHijo("Peso X: " + f.GetPNEquis());
                    nodoFiltroCond.AgregarHijo("Peso 2: " + f.GetPNDoses());
                    AgregarSiTieneValor(nodoFiltroCond, "Tolerancias Peso Global: ", f.GetPNGlobalTol());
                    AgregarSiTieneValor(nodoFiltroCond, "Tolerancias Peso Variantes: ", f.GetPNVariantesTol());
                    AgregarSiTieneValor(nodoFiltroCond, "Tolerancias Peso 1: ", f.GetPNUnosTol());
                    AgregarSiTieneValor(nodoFiltroCond, "Tolerancias Peso X: ", f.GetPNEquisTol());
                    AgregarSiTieneValor(nodoFiltroCond, "Tolerancias Peso 2: ", f.GetPNDosesTol());
                    AgregarSiTieneValor(nodoFiltroCond, "Número de Tolerancias: ", f.GetTolerancias());
                    break;
                }

                // ---- Valoración de Signos — legacy líneas 399-426 ----
                case "ValoracionSignos":
                {
                    var f = (FiltroValoracionSignos)grupo.GetFiltro(Filtro.ValoracionSignos.ToString());
                    var nodoValoracion = nodoFiltroCond.AgregarHijo("Valoración");
                    for (int z = 0; z < f.Valores1.Length; z++)
                    {
                        nodoValoracion.AgregarHijo(f.Valores1[z] + "-" + f.ValoresX[z] + "-" + f.Valores2[z]);
                    }
                    nodoFiltroCond.AgregarHijo("Límites Valoración Global: " + f.ValorGlobal);
                    nodoFiltroCond.AgregarHijo("Límites Valoración 1: " + f.ValorUnos);
                    nodoFiltroCond.AgregarHijo("Límites Valoración X: " + f.ValorEquis);
                    nodoFiltroCond.AgregarHijo("Límites Valoración 2: " + f.ValorDoses);
                    nodoFiltroCond.AgregarHijo("Tipo de Valoración: " + f.TipoValoracion);
                    break;
                }

                // ---- Distancias — legacy líneas 429-442 ----
                case "Distancias":
                {
                    var f = (FiltroDistancias)grupo.GetFiltro(Filtro.Distancias.ToString());
                    nodoFiltroCond.AgregarHijo("Distancias de Variantes: " + f.GetIntVar());
                    nodoFiltroCond.AgregarHijo("Distancias de 1: " + f.GetInt1());
                    nodoFiltroCond.AgregarHijo("Distancias de X: " + f.GetIntX());
                    nodoFiltroCond.AgregarHijo("Distancias de 2: " + f.GetInt2());
                    break;
                }

                // ---- Grupos de Equipos — legacy líneas 445-517 ----
                case "GruposEquipos":
                {
                    var f = (FiltroGruposEquipos)grupo.GetFiltro(Filtro.GruposEquipos.ToString());
                    ConstruirDetalleGruposEquipos(f, nodoFiltroCond);
                    break;
                }

                // ---- Contactos — legacy líneas 520-545 ----
                case "Contactos":
                {
                    var f = (FiltroContactos)grupo.GetFiltro(Filtro.Contactos.ToString());
                    nodoFiltroCond.AgregarHijo("Contactos 1X: " + f.GetNum1X());
                    nodoFiltroCond.AgregarHijo("Contactos 12: " + f.GetNum12());
                    nodoFiltroCond.AgregarHijo("Contactos X2: " + f.GetNumX2());
                    nodoFiltroCond.AgregarHijo("Contactos 11: " + f.GetNum11());
                    nodoFiltroCond.AgregarHijo("Contactos XX: " + f.GetNumXX());
                    nodoFiltroCond.AgregarHijo("Contactos 22: " + f.GetNum22());
                    nodoFiltroCond.AgregarHijo("Contactos 1V: " + f.GetNum1V());
                    nodoFiltroCond.AgregarHijo("Contactos XV: " + f.GetNumXV());
                    nodoFiltroCond.AgregarHijo("Contactos 2V: " + f.GetNum2V());
                    nodoFiltroCond.AgregarHijo("Contactos VV: " + f.GetNumVV());
                    break;
                }

                // ---- Formatos de Signos — legacy líneas 548-575 ----
                case "FormatosSignos":
                {
                    var f = (FiltroFormatosSignos)grupo.GetFiltro(Filtro.FormatosSignos.ToString());
                    ConstruirDetalleFormatosSignos(f, nodoFiltroCond);
                    break;
                }

                // ---- Formatos 123 — legacy líneas 578-622 ----
                case "Formatos123":
                {
                    var f = (FiltroFormatos123)grupo.GetFiltro(Filtro.Formatos123.ToString());
                    ConstruirDetalleFormatos123(f, nodoFiltroCond);
                    break;
                }

                // ---- Simetrías — legacy líneas 625-639 ----
                case "Simetrias":
                {
                    var f = (FiltroSimetrias)grupo.GetFiltro(Filtro.Simetrias.ToString());
                    var nodoSimetrias = nodoFiltroCond.AgregarHijo("Simetrías");
                    foreach (Simetria sim in f.ArraySimetrias)
                    {
                        nodoSimetrias.AgregarHijo("Simetría: " + sim.Partidos);
                    }
                    nodoFiltroCond.AgregarHijo("Aciertos: " + f.Aciertos);
                    break;
                }

                // ---- Diferencias — legacy líneas 642-678 ----
                case "Diferencias":
                {
                    var f = (FiltroDiferencias)grupo.GetFiltro(Filtro.Diferencias.ToString());
                    ConstruirDetalleDiferencias(f, nodoFiltroCond);
                    break;
                }
            }
        }

        /// <summary>Obtiene el patrón de signos de una Columna Probable ("*" para huecos).
        /// Port de ListadoCondicionesFrm.ObtenerStringPronosticoCP (legacy líneas 61-79).</summary>
        private static string ObtenerStringPronosticoCP(ColumnaProbable columna)
        {
            string signosCP = "";
            for (int l = 0; l < columna.Pronosticos.Length; l++)
            {
                signosCP += columna.Pronosticos[l] != "" ? columna.Pronosticos[l] : "*";
                if (l < columna.Pronosticos.Length - 1) signosCP += ",";
            }
            return signosCP;
        }

        /// <summary>Detalle de Columnas Probables: columnas + Relaciones I/II/III + Control de Fallos.
        /// Port de legacy líneas 180-335.</summary>
        private static void ConstruirDetalleColProbables(FiltroColProbables f, CondicionNodo nodoFiltroCond)
        {
            // Columnas Probables (legacy 183-214)
            var nodoCP = nodoFiltroCond.AgregarHijo("Columnas Probables");
            foreach (ColumnaProbable columna in f.ColProbables)
            {
                var nodoCPs = nodoCP.AgregarHijo("Columna Probable: " + ObtenerStringPronosticoCP(columna));
                AgregarSiTieneValor(nodoCPs, "Aciertos: ", columna.GetAciertos());
                AgregarSiTieneValor(nodoCPs, "Aciertos Seguidos: ", columna.GetAciertosSeguidos());
                AgregarSiTieneValor(nodoCPs, "Fallos Seguidos: ", columna.GetFallosSeguidos());
                AgregarSiTieneValor(nodoCPs, "Tolerancias Aciertos: ", columna.GetACTol());
                AgregarSiTieneValor(nodoCPs, "Tolerancias Aciertos Seguidos: ", columna.GetACSTol());
                AgregarSiTieneValor(nodoCPs, "Tolerancias Fallos Seguidos: ", columna.GetFSTol());
                AgregarSiTieneValor(nodoCPs, "Aciertos en Tolerancias: ", columna.GetTolerancias());
                AgregarSiTieneValor(nodoCPs, "Puntos: ", columna.GetPuntos());
            }

            // Relaciones (CP1) — legacy 216-236
            var nodoRelacionesCP = nodoFiltroCond.AgregarHijo("Relaciones");
            foreach (RelacionCP1 relCP in f.RelacionesCP1.Relaciones)
            {
                var nodoRelacionCP = nodoRelacionesCP.AgregarHijo("Columnas: " + relCP.Columnas);
                // El legacy etiqueta "Aciertos" con relCP.CantidadCP (no SumaAciertos): se replica tal cual.
                nodoRelacionCP.AgregarHijo("Cuantas: " + relCP.CantidadCP);
                nodoRelacionCP.AgregarHijo("Aciertos: " + relCP.CantidadCP);
                nodoRelacionCP.AgregarHijo("Suma Aciertos: " + relCP.SumaAciertos);
                nodoRelacionCP.AgregarHijo("Recorrido: " + relCP.Recorridos);
            }

            // Relaciones II (CP2) — legacy 240-253
            var nodoRelacionesCP2 = nodoFiltroCond.AgregarHijo("Relaciones II");
            foreach (RelacionCP2 relCP2 in f.RelacionesCP2.Relaciones2)
            {
                var nodoRelII = nodoRelacionesCP2.AgregarHijo("Relación II");
                nodoRelII.AgregarHijo("Las columnas: " + relCP2.StrColsA + " tendrán " + relCP2.StrAciertos + " " + relCP2.Concepto + " " + relCP2.Cantidad + " que las columnas " + relCP2.StrColsB);
                nodoRelII.AgregarHijo("Las columnas: " + relCP2.StrColsA2 + " tendrán " + relCP2.StrAciertos2 + " " + relCP2.Concepto2 + " " + relCP2.Cantidad2 + " que las columnas " + relCP2.StrColsB2);
            }

            // Relaciones III (CP3) — legacy 256-313
            var nodoRelacionesCP3 = nodoFiltroCond.AgregarHijo("Relaciones III");
            foreach (RelacionCP3 relCP3 in f.RelacionesCP3.Relaciones)
            {
                var nodoRelIII = nodoRelacionesCP3.AgregarHijo("Relación III");
                nodoRelIII.AgregarHijo("Columnas: " + relCP3.ColumnasImplicadasString);
                nodoRelIII.AgregarHijo("Concepto: " + relCP3.ConceptoString);
                nodoRelIII.AgregarHijo("Sandwichs: " + relCP3.NumeroSandwichsPermitidosString);

                var nodoEscaleras = nodoRelIII.AgregarHijo("Escaleras");
                nodoEscaleras.AgregarHijo("Totales: " + relCP3.NumeroEscalerasTotalesPermitidasString);
                nodoEscaleras.AgregarHijo("Ascendentes: " + relCP3.NumeroEscalerasASCPermitidasString);
                nodoEscaleras.AgregarHijo("Descendentes: " + relCP3.NumeroEscalerasDESCPermitidasString);

                // Paso Fijo (legacy 273-287): un único nodo "Agrupación" que acumula las agrupaciones.
                var nodoPasoFijo = nodoRelIII.AgregarHijo("Agrupaciones Paso Fijo");
                var nodoAgrupacionPasoFijo = nodoPasoFijo.AgregarHijo("Agrupación");
                foreach (AgrupacionColumnas agrup in relCP3.AgrupacionesPasoFijoPermitidas)
                {
                    nodoAgrupacionPasoFijo.AgregarHijo("Número: " + UtilidadesEntradasValores.ObtenerTextoFromListAciertos(agrup.Numero));
                    nodoAgrupacionPasoFijo.AgregarHijo("Elementos: " + agrup.NoElementos);
                    nodoAgrupacionPasoFijo.AgregarHijo("Aciertos: " + UtilidadesEntradasValores.ObtenerTextoFromListAciertos(agrup.NoAciertos));
                }

                // Solapadas (legacy 289-303)
                var nodoSolapadas = nodoRelIII.AgregarHijo("Agrupaciones Solapadas");
                var nodoAgrupacionSol = nodoSolapadas.AgregarHijo("Agrupación");
                foreach (AgrupacionColumnas agrup in relCP3.AgrupacionesSolapadasPermitidas)
                {
                    nodoAgrupacionSol.AgregarHijo("Número: " + UtilidadesEntradasValores.ObtenerTextoFromListAciertos(agrup.Numero));
                    nodoAgrupacionSol.AgregarHijo("Elementos: " + agrup.NoElementos);
                    nodoAgrupacionSol.AgregarHijo("Aciertos: " + UtilidadesEntradasValores.ObtenerTextoFromListAciertos(agrup.NoAciertos));
                }
            }

            // Control de Fallos — legacy 316-334
            var nodoControlFallosCP = nodoFiltroCond.AgregarHijo("Control de Fallos");
            foreach (CPControlFallos controlFallosCP in f.ControlFallosCP.ControlesFallos)
            {
                var nodoControlesFallosCP = nodoControlFallosCP.AgregarHijo("Control Fallos");
                // Orden del legacy: Columnas, Tolerancia, Aciertos.
                nodoControlesFallosCP.AgregarHijo("Columnas: " + controlFallosCP.Columnas);
                nodoControlesFallosCP.AgregarHijo("Tolerancia: " + controlFallosCP.Tolerancias);
                nodoControlesFallosCP.AgregarHijo("Aciertos: " + controlFallosCP.Aciertos);
            }
            nodoControlFallosCP.AgregarHijo("Numero de Fallos de Controles: " + f.ControlFallosCP.FallosPermitidos);
        }

        /// <summary>Detalle de Grupos de Equipos: conjuntos (marcados, V/E/D, puntos) + relaciones.
        /// Port de legacy líneas 445-517.</summary>
        private static void ConstruirDetalleGruposEquipos(FiltroGruposEquipos f, CondicionNodo nodoFiltroCond)
        {
            // El legacy crea "Relaciones" (nodoRelacionesGE) primero, añade los conjuntos directamente al
            // filtro, y al final cuelga "Relaciones" con sus relaciones. Se mantiene ese mismo orden.
            var nodoRelacionesGE = new CondicionNodo("Relaciones");

            for (int z = 0; z < f.GruposEquipos.Count; z++)
            {
                var nodoConjuntoGE = nodoFiltroCond.AgregarHijo("Conjunto " + (z + 1));
                var nodoEquiposMarcados = nodoConjuntoGE.AgregarHijo("Equipos Marcados");
                GrupoEquipos grupoE = f.GruposEquipos[z];
                foreach (char marcados in grupoE.Pronosticos)
                {
                    switch (marcados)
                    {
                        case '1': nodoEquiposMarcados.AgregarHijo("Marcado - Desmarcado"); break;
                        case '2': nodoEquiposMarcados.AgregarHijo("Desmarcado - Marcado"); break;
                        case '3': nodoEquiposMarcados.AgregarHijo("Marcado - Marcado"); break;
                        case '0': nodoEquiposMarcados.AgregarHijo("Desmarcado - Desmarcado"); break;
                    }
                }
                nodoConjuntoGE.AgregarHijo("Victorias: " + grupoE.Victorias);
                nodoConjuntoGE.AgregarHijo("Empates: " + grupoE.Empates);
                nodoConjuntoGE.AgregarHijo("Derrotas: " + grupoE.Derrotas);
                nodoConjuntoGE.AgregarHijo("Suma Puntos: " + grupoE.SumaPuntos);
            }

            foreach (RelacionGE1 relGE in f.RelacionesGE1.Relaciones)
            {
                var nodoRelacionGE = nodoRelacionesGE.AgregarHijo("Grupos de Equipos: " + relGE.GruposEquipos);
                nodoRelacionGE.AgregarHijo("Suma Victorias: " + relGE.SumaVictorias);
                nodoRelacionGE.AgregarHijo("Suma Empates: " + relGE.SumaEmpates);
                nodoRelacionGE.AgregarHijo("Suma Derrotas: " + relGE.SumaDerrotas);
                nodoRelacionGE.AgregarHijo("Suma Puntos: " + relGE.SumaPuntos);
            }
            nodoFiltroCond.Hijos.Add(nodoRelacionesGE);
        }

        /// <summary>Detalle de Formatos de Signos: conjuntos, líneas de formato y rangos.
        /// Port de legacy líneas 548-575.</summary>
        private static void ConstruirDetalleFormatosSignos(FiltroFormatosSignos f, CondicionNodo nodoFiltroCond)
        {
            var nodoFormatoSignos = nodoFiltroCond.AgregarHijo("Conjuntos");
            foreach (FormatosSignos formatosSignos in f.FormatosSignos)
            {
                var nodoConjuntoFormatos = new CondicionNodo("Conjunto");
                foreach (FormatoSignos formatoSignos in formatosSignos.LineasFormatos)
                {
                    var nodoFormato = nodoConjuntoFormatos.AgregarHijo("Formato: " + formatoSignos.Formato);
                    nodoFormato.AgregarHijo("Rango: " + formatoSignos.RangoAparicion);
                    // El legacy añade el conjunto al árbol dentro del bucle de líneas (Nodes.Add idempotente
                    // sobre el mismo nodo); aquí se añade una sola vez al cerrar el conjunto, equivalente.
                }
                // legacy: nodoFormatosGlobal usa formatosSignos.Lineas (mismo valor que "Lineas"), se replica.
                nodoConjuntoFormatos.AgregarHijo("Lineas: " + formatosSignos.Lineas);
                nodoConjuntoFormatos.AgregarHijo("Global: " + formatosSignos.Lineas);
                nodoFormatoSignos.Hijos.Add(nodoConjuntoFormatos);
            }
        }

        /// <summary>Detalle de Formatos 123: valoración, formatos y modo (paso fijo / repeticiones).
        /// Port de legacy líneas 578-622.</summary>
        private static void ConstruirDetalleFormatos123(FiltroFormatos123 f, CondicionNodo nodoFiltroCond)
        {
            var nodoValoracion123 = nodoFiltroCond.AgregarHijo("Valoración");
            for (int z = 0; z < f.Valores.Length / 3; z++)
            {
                nodoValoracion123.AgregarHijo(f.Valores[z, 0] + "-" + f.Valores[z, 1] + "-" + f.Valores[z, 2]);
            }

            var nodoFormatos123 = nodoFiltroCond.AgregarHijo("Formatos");
            foreach (Formato123 formato123 in f.ArrayFormatos)
            {
                var nodoFormato123 = nodoFormatos123.AgregarHijo("Formato 123: " + formato123.Formato);
                if (!f.PasoFijo)
                {
                    nodoFormato123.AgregarHijo("Límites: " + formato123.AciertosMin + "-" + formato123.AciertosMax);
                }
            }
            if (f.PasoFijo)
            {
                nodoFormatos123.AgregarHijo("Aciertos Globales: " + f.AciertosFiltro);
                nodoFormatos123.AgregarHijo("Modo: No Admitiendo Repeticiones");
            }
            else
            {
                nodoFormatos123.AgregarHijo("Líneas Acertadas: " + f.AciertosFiltro);
                nodoFormatos123.AgregarHijo("Modo: Admitiendo Repeticiones");
            }
        }

        /// <summary>Detalle de Diferencias: partidos simétricos + qué conceptos se comparan.
        /// Port de legacy líneas 642-678.</summary>
        private static void ConstruirDetalleDiferencias(FiltroDiferencias f, CondicionNodo nodoFiltroCond)
        {
            for (int z = 0; z < f.Diferencias.Count; z++)
            {
                var nodoDiferencia = nodoFiltroCond.AgregarHijo("Diferencia " + (z + 1));
                Diferencia dif = f.Diferencias[z];
                foreach (bool[] partidos in dif.PartidosSimetricos)
                {
                    nodoDiferencia.AgregarHijo("Partidos: " + UtilidadesEntradasValores.ObtenerTextoFromBool(partidos));
                }

                var nodoDiferenciasValores = nodoDiferencia.AgregarHijo("Diferencias");
                nodoDiferenciasValores.AgregarHijo("Variantes: " + UtilidadesEntradasValores.ObtenerTextoFromBool(dif.AcV));
                nodoDiferenciasValores.AgregarHijo("Equis: " + UtilidadesEntradasValores.ObtenerTextoFromBool(dif.AcX));
                nodoDiferenciasValores.AgregarHijo("Doses: " + UtilidadesEntradasValores.ObtenerTextoFromBool(dif.AcDoses));
                nodoDiferenciasValores.AgregarHijo("Dibujos: " + UtilidadesEntradasValores.ObtenerTextoFromBool(dif.AcDib));
                nodoDiferenciasValores.AgregarHijo("Interrupciones: " + UtilidadesEntradasValores.ObtenerTextoFromBool(dif.AcInt));
                nodoDiferenciasValores.AgregarHijo("Formatos: " + UtilidadesEntradasValores.ObtenerTextoFromBool(dif.AcFormatos));
            }
        }

        /// <summary>Añade "prefijo + valor" sólo si el valor no está vacío, replicando las guardas
        /// `if (nodo.Text != "prefijo")` del legacy (p.ej. tolerancias / aciertos opcionales).</summary>
        private static void AgregarSiTieneValor(CondicionNodo padre, string prefijo, string valor)
        {
            if (!string.IsNullOrEmpty(valor))
            {
                padre.AgregarHijo(prefijo + valor);
            }
        }

        [RelayCommand]
        private void ExpandirTodo()
        {
            SetExpandedRecursivo(Condiciones, true);
        }

        [RelayCommand]
        private void ColapsarTodo()
        {
            SetExpandedRecursivo(Condiciones, false);
        }

        [RelayCommand]
        private async Task ExportarTexto()
        {
            // Equivale a ListadoCondicionesFrm.Exportar_Click(): texto plano del árbol -> .txt + abrir.
            if (Condiciones.Count == 0) return;
            string texto = ExportarATexto(Condiciones[0], 0);
            await GuardarYAbrirAsync(texto, "txt", "Listados");
        }

        [RelayCommand]
        private async Task ExportarHtml()
        {
            // Equivale a ListadoCondicionesFrm.ExportarHtml_Click(): HTML del árbol -> .html + abrir.
            if (Condiciones.Count == 0) return;
            string texto = ExportarAHtml(Condiciones[0], 0);
            texto = "<html><head><title>Listado de Condiciones</title></head><body bgcolor=\"#DBFEBC\">"
                    + texto + "</body></html>";
            await GuardarYAbrirAsync(texto, "html", "Listados");
        }

        // ======================= Exportación (recursión legacy) =======================

        private static string ExportarATexto(CondicionNodo nodo, int profundidad)
        {
            // Equivale a ListadoCondicionesFrm.ExportarATexto(TreeNode, profundidad).
            var sb = new StringBuilder();
            foreach (CondicionNodo hijo in nodo.Hijos)
            {
                if (hijo.Hijos.Count > 0)
                {
                    sb.Append(new string(' ', profundidad));
                    sb.Append(hijo.Texto);
                    sb.Append("\r\n\r\n");
                    sb.Append(ExportarATexto(hijo, profundidad + 1));
                    sb.Append("\r\n");
                }
                else
                {
                    sb.Append(new string(' ', profundidad));
                    sb.Append(hijo.Texto);
                    sb.Append("\r\n");
                }
            }
            return sb.ToString();
        }

        private static string ExportarAHtml(CondicionNodo nodo, int profundidad)
        {
            // Equivale a ListadoCondicionesFrm.ExportarAHtml(TreeNode, profundidad).
            var sb = new StringBuilder();
            foreach (CondicionNodo hijo in nodo.Hijos)
            {
                if (hijo.Hijos.Count > 0)
                {
                    if (profundidad == 0) sb.Append("<br><b>");
                    else if (profundidad == 1) sb.Append("<i>");
                    sb.Append(hijo.Texto);
                    if (profundidad == 0) sb.Append("</b><br>");
                    else if (profundidad == 1) sb.Append("</i>");
                    sb.Append("<br>");
                    sb.Append(ExportarAHtml(hijo, profundidad + 1));
                }
                else
                {
                    sb.Append(hijo.Texto);
                    sb.Append("<br>");
                }
            }
            return sb.ToString();
        }

        private static async Task GuardarYAbrirAsync(string contenido, string extension, string nombreSugerido)
        {
            var picker = new FileSavePicker
            {
                SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
                SuggestedFileName = nombreSugerido,
            };
            picker.FileTypeChoices.Add("Listados", new List<string> { "." + extension });
            WinRT.Interop.InitializeWithWindow.Initialize(picker, AppServices.WindowHandle);

            StorageFile? file = await picker.PickSaveFileAsync();
            if (file == null) return;

            try
            {
                await FileIO.WriteTextAsync(file, contenido, Windows.Storage.Streams.UnicodeEncoding.Utf8);
                // legacy: Process.Start(nombreArchivo) -> abrir con la app asociada.
                await Launcher.LaunchFileAsync(file);
            }
            catch (Exception ex)
            {
                AppServices.MostrarError("No se pudo exportar: " + ex.Message);
            }
        }

        private static void SetExpandedRecursivo(ObservableCollection<CondicionNodo> nodos, bool expandido)
        {
            foreach (CondicionNodo nodo in nodos)
            {
                nodo.IsExpanded = expandido;
                SetExpandedRecursivo(nodo.Hijos, expandido);
            }
        }
    }
}
