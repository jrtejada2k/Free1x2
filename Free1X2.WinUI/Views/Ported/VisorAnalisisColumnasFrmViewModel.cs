using System.Collections.Generic;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Free1X2.Analisis;
using Free1X2.MotorCalculo;
using Free1X2.WinUI.Controls.Analisis;
using Microsoft.UI.Xaml;

namespace Free1X2.WinUI.Views.Ported
{
    // Port of legacy WinForms "VisorAnalisisColumnasFrm" (Free1X2.UI.VisorAnalisisColumnasFrm).
    // El legacy es un visor de solo lectura con pestañas (TabControl); las pestañas visibles
    // dependen de los flags VariablesGlobales.Analizar* y cada pestaña aloja un UserControl de
    // análisis (CtrlAnalisisVX2, CtrlAnalisisSSeguidos, CtrlAnalisisSimetrias, CtrlDataGridViewCPs,
    // CtrlAnalisisValoraciones, CtrlAnalisisFormatosSignos, ...).
    //
    // Cableado al motor real: lee el handoff estático (UltimoContenedor = ContenedorAnalisisGlobal,
    // UltimoGrupo = Grupo) que deja el hook App.CablearHooksDominio (AnalisisUi.MostrarVisor) y
    // construye la lista de secciones activas a partir de VariablesGlobales.Analizar*.
    //
    // Presentación enriquecida (este lote): en vez de volcar las matrices del ContenedorAnalisis
    // a texto plano, se construyen modelos estructurados (MatrizAnalisis / ColumnaValoracion /
    // entradas valor->columnas) que la página pinta con controles nativos (MatrizAnalisisView,
    // expanders, rejillas), replicando 1:1 la información que mostraban los CtrlAnalisis* del WinForms.
    // NUNCA se fabrican datos: todos los valores provienen del ContenedorAnalisis del dominio.

    /// <summary>Tipo de presentación de una sección (determina qué presentador usa la página).</summary>
    public enum TipoSeccionAnalisis
    {
        /// <summary>Sin datos / mensaje informativo (sólo Detalle).</summary>
        Vacia,
        /// <summary>Una o varias matrices con cabecera de índices + filas de conceptos.</summary>
        Matrices,
        /// <summary>Cuatro columnas de valoración (Global/Unos/Equis/Doses).</summary>
        Valoracion,
    }

    // One analysis section (mirrors a legacy TabPage + its custom control).
    public partial class SeccionAnalisisItem : ObservableObject
    {
        public SeccionAnalisisItem(string clave, string titulo)
        {
            _clave = clave;
            _titulo = titulo;
        }

        [ObservableProperty]
        private string _clave;

        [ObservableProperty]
        private string _titulo;

        // Texto descriptivo / estado (p. ej. "No hay datos para analizar").
        [ObservableProperty]
        private string _detalle = "Datos de análisis cargados desde el dominio.";

        // Si el análisis correspondiente está activo (VariablesGlobales.Analizar*).
        [ObservableProperty]
        private bool _activo = true;

        /// <summary>Tipo de presentación de la sección.</summary>
        public TipoSeccionAnalisis Tipo { get; set; } = TipoSeccionAnalisis.Vacia;

        /// <summary>
        /// Sub-bloques de matrices de la sección. Para VX2/Seguidos/Contactos/... hay un único
        /// bloque; para CPs hay tres (Aciertos / Aciertos seguidos / Fallos seguidos); para
        /// Diferencias y Formatos hay uno por conjunto/grupo. Cada bloque lleva su propio título.
        /// </summary>
        public ObservableCollection<BloqueMatrizAnalisis> Bloques { get; } = new();

        /// <summary>Columnas de valoración (sólo cuando Tipo == Valoracion).</summary>
        public ObservableCollection<ColumnaValoracion> ColumnasValoracion { get; } = new();

        // Marcador textual del estado (string para TextBlock.Text; regla anti-crash: no bindear bool).
        public string EstadoMarcador => Activo ? "●" : string.Empty;

        partial void OnActivoChanged(bool value)
        {
            OnPropertyChanged(nameof(EstadoMarcador));
        }
    }

    /// <summary>Un bloque de matriz con su título (réplica de un CtrlAnalisis* o de un grupo/conjunto).</summary>
    public sealed class BloqueMatrizAnalisis
    {
        public BloqueMatrizAnalisis(string titulo, MatrizAnalisis matriz)
        {
            Titulo = titulo;
            Matriz = matriz;
        }

        public string Titulo { get; }
        public MatrizAnalisis Matriz { get; }
    }

    public partial class VisorAnalisisColumnasFrmViewModel : ObservableObject
    {
        // Payload que el dominio entrega a través del hook AnalisisUi.MostrarVisor
        // (contenedor = Free1X2.Analisis.ContenedorAnalisisGlobal, grupo = Free1X2.MotorCalculo.Grupo).
        // Se guarda de forma estática hasta que el visor lo consume al navegar.
        public static object? UltimoContenedor { get; set; }
        public static object? UltimoGrupo { get; set; }

        private readonly ContenedorAnalisisGlobal? _contenedor;

        public VisorAnalisisColumnasFrmViewModel()
        {
            _contenedor = UltimoContenedor as ContenedorAnalisisGlobal;

            Secciones = new ObservableCollection<SeccionAnalisisItem>();
            ConstruirSecciones();

            _seccionSeleccionada = Secciones.Count > 0 ? Secciones[0] : null;
        }

        public ObservableCollection<SeccionAnalisisItem> Secciones { get; }

        [ObservableProperty]
        private SeccionAnalisisItem? _seccionSeleccionada;

        // Título del detalle = título de la sección seleccionada (string para TextBlock.Text).
        public string DetalleTitulo => SeccionSeleccionada?.Titulo ?? "Selecciona un análisis";

        public string DetalleTexto => SeccionSeleccionada?.Detalle
            ?? "No hay ningún análisis seleccionado.";

        /// <summary>Bloques de matrices de la sección seleccionada (rejillas del detalle).</summary>
        public IReadOnlyList<BloqueMatrizAnalisis> BloquesDetalle =>
            SeccionSeleccionada?.Bloques ?? (IReadOnlyList<BloqueMatrizAnalisis>)System.Array.Empty<BloqueMatrizAnalisis>();

        /// <summary>Columnas de valoración de la sección seleccionada.</summary>
        public IReadOnlyList<ColumnaValoracion> ColumnasValoracionDetalle =>
            SeccionSeleccionada?.ColumnasValoracion ?? (IReadOnlyList<ColumnaValoracion>)System.Array.Empty<ColumnaValoracion>();

        /// <summary>Visibilidad del presentador de matrices según el tipo de la sección seleccionada.</summary>
        public Visibility VisibilidadMatrices =>
            SeccionSeleccionada?.Tipo == TipoSeccionAnalisis.Matrices ? Visibility.Visible : Visibility.Collapsed;

        /// <summary>Visibilidad del presentador de valoración según el tipo de la sección seleccionada.</summary>
        public Visibility VisibilidadValoracion =>
            SeccionSeleccionada?.Tipo == TipoSeccionAnalisis.Valoracion ? Visibility.Visible : Visibility.Collapsed;

        partial void OnSeccionSeleccionadaChanged(SeccionAnalisisItem? value)
        {
            OnPropertyChanged(nameof(DetalleTitulo));
            OnPropertyChanged(nameof(DetalleTexto));
            OnPropertyChanged(nameof(BloquesDetalle));
            OnPropertyChanged(nameof(ColumnasValoracionDetalle));
            OnPropertyChanged(nameof(VisibilidadMatrices));
            OnPropertyChanged(nameof(VisibilidadValoracion));
        }

        [RelayCommand]
        private void Refrescar()
        {
            // Legacy: MostrarDatos(grupoActual) reconstruía las pestañas según VariablesGlobales.Analizar*.
            var seleccionada = SeccionSeleccionada?.Clave;
            ConstruirSecciones();
            SeccionSeleccionada = null;
            foreach (var s in Secciones)
            {
                if (s.Clave == seleccionada) { SeccionSeleccionada = s; break; }
            }
            if (SeccionSeleccionada == null && Secciones.Count > 0)
            {
                SeccionSeleccionada = Secciones[0];
            }
        }

        /// <summary>
        /// Reconstruye la lista de secciones activas (legacy: MostrarDatos -> AñadirX()/BorrarTab()
        /// según VariablesGlobales.Analizar*), volcando los datos del ContenedorAnalisis.
        /// </summary>
        private void ConstruirSecciones()
        {
            Secciones.Clear();
            ContenedorAnalisis? c = _contenedor?.AnalisisGrupos;

            // VX2: 3 conceptos (Variantes/Equis/Doses) x (NumeroPartidos+1) valores.
            // Legacy: CtrlAnalisisVX2 (filas lblVariantes/lblEquis/lblDoses + cabecera de índices).
            if (VariablesGlobales.AnalizarVX2)
                Secciones.Add(SeccionMatriz("VX2", "VX2 — Variantes, Equis, Doses",
                    c?.VX2, new[] { "Variantes", "Equis", "Doses" }));

            // Signos Seguidos. Legacy: CtrlAnalisisSSeguidos.
            if (VariablesGlobales.AnalizarSeguidos)
                Secciones.Add(SeccionMatriz("SignosSeguidos", "Signos Seguidos",
                    c?.Seguidos, new[] { "Variantes seguidas", "Unos seguidos", "Equis seguidas", "Doses seguidos" }));

            // Dibujos. Legacy: CtrlDibujos (matriz NoEquis x NoDoses).
            if (VariablesGlobales.AnalizarDibujos)
                Secciones.Add(SeccionMatriz("Dibujos", "Dibujos", c?.Dibujos, null));

            // Interrupciones. Legacy: CtrlAnalisisInterrupciones (10 conceptos).
            if (VariablesGlobales.AnalizarInterrupciones)
                Secciones.Add(SeccionMatriz("Interrupciones", "Interrupciones", c?.Interrupciones, new[]
                {
                    "Globales", "Variantes", "Unos", "Equis", "Doses",
                    "Globales seguidas", "Variantes seguidas", "Unos seguidos", "Equis seguidas", "Doses seguidos"
                }));

            // Contactos. Legacy: CtrlAnalisisContactos (10 conceptos).
            if (VariablesGlobales.AnalizarContactos)
                Secciones.Add(SeccionMatriz("Contactos", "Contactos", c?.Contactos, new[]
                {
                    "1X", "12", "X2", "11", "XX", "22", "1V", "XV", "2V", "VV"
                }));

            // Pesos numéricos. Legacy: CtrlAnalisisPesos (5 conceptos).
            if (VariablesGlobales.AnalizarPesos)
                Secciones.Add(SeccionMatriz("Pesos", "Pesos Numéricos", c?.Pesos, new[]
                {
                    "Global", "Variantes", "Unos", "Equis", "Doses"
                }));

            // Distancias. Legacy: CtrlAnalisisDistancias (4 conceptos).
            if (VariablesGlobales.AnalizarDistancias)
                Secciones.Add(SeccionMatriz("Distancias", "Distancias", c?.Distancias, new[]
                {
                    "Variantes", "Unos", "Equis", "Doses"
                }));

            // Control de grupos / conjuntos: vector int[] columnas por nº de fallos.
            // Legacy: CtrlAnalisisControlGrupos / CtrlAnalisisControlConjuntos.
            if (VariablesGlobales.AnalizarControlGrupos)
                Secciones.Add(SeccionVector("ControlGrupos", "Control Grupos",
                    _contenedor?.ColumnasPorFallosDeGrupos, "Fallos", "Columnas"));

            if (VariablesGlobales.AnalizarControlConjuntos)
                Secciones.Add(SeccionVector("ControlConjuntos", "Control Conjuntos",
                    _contenedor?.ColumnasPorFallosDeConjuntos, "Fallos", "Columnas"));

            // Simetrías. Legacy: CtrlAnalisisSimetrias (fila Aciertos + fila Columnas).
            if (VariablesGlobales.AnalizarSimetrias)
                Secciones.Add(SeccionSimetrias("Simetrias", "Simetrías", c));

            // Diferencias. Legacy: CtrlAnalisisDiferencias (un CtrlAnalisisDiferencias_Individual por conjunto).
            if (VariablesGlobales.AnalizarSimetriasII)
                Secciones.Add(SeccionDiferencias("Diferencias", "Diferencias", c));

            // Valoración. Legacy: CtrlAnalisisValoraciones (4 columnas Global/Unos/Equis/Doses).
            if (VariablesGlobales.AnalizarValoracion)
                Secciones.Add(SeccionValoracion("Valoracion", "Valoración", c));

            // Columnas probables. Legacy: CtrlDataGridViewCPs (3 rejillas: AC / ACS / FS).
            if (VariablesGlobales.AnalizarCPs)
                Secciones.Add(SeccionCPs("ColumnasProbables", "Columnas Probables", c));

            // Formatos. Legacy: CtrlAnalisisFormatosSignos (tablas Líneas y Globales por grupo).
            if (VariablesGlobales.AnalizarFormatos)
                Secciones.Add(SeccionFormatos("Formatos", "Formatos", c));

            if (Secciones.Count == 0)
            {
                var vacia = new SeccionAnalisisItem("SinAnalisis", "Sin análisis activos")
                {
                    Tipo = TipoSeccionAnalisis.Vacia,
                    Detalle = _contenedor == null
                        ? "No se ha recibido ningún análisis. Ejecuta un análisis para ver los datos."
                        : "No hay ningún tipo de análisis activo (VariablesGlobales.Analizar*)."
                };
                Secciones.Add(vacia);
            }
        }

        // ===== Constructores de secciones =====

        // Matriz int[fila, columna] -> bloque con cabecera de índices (0..cols-1) + 1 fila por concepto.
        // Replica CtrlAnalisisVX2/SSeguidos/Interrupciones/Contactos/Pesos/Distancias/Dibujos.
        private static SeccionAnalisisItem SeccionMatriz(string clave, string titulo, int[,]? matriz, string[]? etiquetas)
        {
            var s = new SeccionAnalisisItem(clave, titulo) { Tipo = TipoSeccionAnalisis.Matrices };
            if (matriz == null)
            {
                s.Tipo = TipoSeccionAnalisis.Vacia;
                s.Detalle = titulo + ": no hay datos para analizar";
                return s;
            }
            int filas = matriz.GetLength(0);
            int cols = matriz.GetLength(1);
            s.Detalle = $"{titulo}: {filas} conceptos × {cols} valores (cada celda = nº de columnas).";
            s.Bloques.Add(new BloqueMatrizAnalisis(string.Empty, ConstruirMatriz(matriz, etiquetas)));
            return s;
        }

        // Construye una MatrizAnalisis: cabecera de índices + filas etiquetadas.
        private static MatrizAnalisis ConstruirMatriz(int[,] matriz, string[]? etiquetas)
        {
            int filas = matriz.GetLength(0);
            int cols = matriz.GetLength(1);

            var filasModelo = new List<FilaMatrizAnalisis>(filas + 1);

            // Cabecera de índices (legacy: LlenarNumeros con CtrlCasilla NavajoWhite).
            var celdasCab = new List<CeldaAnalisis>(cols);
            for (int j = 0; j < cols; j++)
            {
                celdasCab.Add(new CeldaAnalisis(j.ToString(), esCabecera: true));
            }
            filasModelo.Add(new FilaMatrizAnalisis(string.Empty, celdasCab, esCabecera: true));

            // Filas de conceptos.
            for (int i = 0; i < filas; i++)
            {
                var celdas = new List<CeldaAnalisis>(cols);
                for (int j = 0; j < cols; j++)
                {
                    celdas.Add(new CeldaAnalisis(matriz[i, j].ToString()));
                }
                string etiqueta = (etiquetas != null && i < etiquetas.Length) ? etiquetas[i] : "Fila " + i;
                filasModelo.Add(new FilaMatrizAnalisis(etiqueta, celdas));
            }

            return new MatrizAnalisis(filasModelo);
        }

        // Vector int[] (columnas por nº de fallos) -> matriz de 2 filas (índices + valores).
        private static SeccionAnalisisItem SeccionVector(string clave, string titulo, int[]? vector,
            string etiquetaIndices, string etiquetaValores)
        {
            var s = new SeccionAnalisisItem(clave, titulo) { Tipo = TipoSeccionAnalisis.Matrices };
            if (vector == null)
            {
                s.Tipo = TipoSeccionAnalisis.Vacia;
                s.Detalle = titulo + ": no hay datos para analizar";
                return s;
            }
            s.Detalle = $"{titulo}: columnas por nº de fallos ({vector.Length} valores).";

            var filas = new List<FilaMatrizAnalisis>(2);
            var cabecera = new List<CeldaAnalisis>(vector.Length);
            var valores = new List<CeldaAnalisis>(vector.Length);
            for (int i = 0; i < vector.Length; i++)
            {
                cabecera.Add(new CeldaAnalisis(i.ToString(), esCabecera: true));
                valores.Add(new CeldaAnalisis(vector[i].ToString()));
            }
            filas.Add(new FilaMatrizAnalisis(etiquetaIndices, cabecera, esCabecera: true));
            filas.Add(new FilaMatrizAnalisis(etiquetaValores, valores));
            s.Bloques.Add(new BloqueMatrizAnalisis(string.Empty, new MatrizAnalisis(filas)));
            return s;
        }

        // Simetrías: una matriz de 2 filas (Aciertos -> cabecera, Columnas -> valores).
        // Legacy: CtrlAnalisisSimetrias (LlenarNumeros + LlenarSimetrias).
        private static SeccionAnalisisItem SeccionSimetrias(string clave, string titulo, ContenedorAnalisis? c)
        {
            var s = new SeccionAnalisisItem(clave, titulo) { Tipo = TipoSeccionAnalisis.Matrices };
            var lista = c?.ContenedorSim;
            if (lista == null || lista.Count == 0)
            {
                s.Tipo = TipoSeccionAnalisis.Vacia;
                s.Detalle = "Simetrías: No hay datos para analizar";
                return s;
            }
            s.Detalle = $"Simetrías: {lista.Count} grupos de aciertos.";

            var cabecera = new List<CeldaAnalisis>(lista.Count);
            var valores = new List<CeldaAnalisis>(lista.Count);
            for (int i = 0; i < lista.Count; i++)
            {
                ContenedorSimetrias cs = lista[i];
                cabecera.Add(new CeldaAnalisis(cs.Aciertos.ToString(), esCabecera: true));
                valores.Add(new CeldaAnalisis(cs.Columnas.ToString()));
            }
            var filas = new List<FilaMatrizAnalisis>
            {
                new FilaMatrizAnalisis("Nº Aciertos", cabecera, esCabecera: true),
                new FilaMatrizAnalisis("Columnas", valores),
            };
            s.Bloques.Add(new BloqueMatrizAnalisis(string.Empty, new MatrizAnalisis(filas)));
            return s;
        }

        // Diferencias: un bloque ("Conjunto n") por contenedor, cada uno con cabecera de índices
        // (1..N) + 6 filas (Variantes/Equis/Dibujos/Doses/Interrupciones/Formatos).
        // Legacy: CtrlAnalisisDiferencias -> CtrlAnalisisDiferencias_Individual (i va de 1 a cuantos-1).
        private static SeccionAnalisisItem SeccionDiferencias(string clave, string titulo, ContenedorAnalisis? c)
        {
            var s = new SeccionAnalisisItem(clave, titulo) { Tipo = TipoSeccionAnalisis.Matrices };
            var lista = c?.ContenedorDiferencias;
            if (lista == null || lista.Count == 0)
            {
                s.Tipo = TipoSeccionAnalisis.Vacia;
                s.Detalle = "Diferencias: No hay datos para analizar";
                return s;
            }
            s.Detalle = $"Diferencias: {lista.Count} conjuntos.";
            for (int i = 0; i < lista.Count; i++)
            {
                ContenedorDiferencias cd = lista[i];
                int cuantos = cd.NumDiferenciasPosibles; // longitud de los arrays
                var filas = new List<FilaMatrizAnalisis>();

                // Cabecera de índices: legacy recorre i de 1 a cuantos-1.
                var cab = new List<CeldaAnalisis>();
                for (int j = 1; j < cuantos; j++)
                {
                    cab.Add(new CeldaAnalisis(j.ToString(), esCabecera: true));
                }
                filas.Add(new FilaMatrizAnalisis("Nº", cab, esCabecera: true));

                filas.Add(FilaDesdeArray("Variantes", cd.Variantes, cuantos));
                filas.Add(FilaDesdeArray("Equis", cd.Equis, cuantos));
                filas.Add(FilaDesdeArray("Dibujos", cd.Dibujos, cuantos));
                filas.Add(FilaDesdeArray("Doses", cd.Doses, cuantos));
                filas.Add(FilaDesdeArray("Interrupciones", cd.Interrupciones, cuantos));
                filas.Add(FilaDesdeArray("Formatos", cd.Formatos, cuantos));

                s.Bloques.Add(new BloqueMatrizAnalisis("Conjunto " + (i + 1), new MatrizAnalisis(filas)));
            }
            return s;
        }

        // Construye una fila a partir de un int[] recorriendo i de 1 a cuantos-1 (igual que el legacy).
        private static FilaMatrizAnalisis FilaDesdeArray(string etiqueta, int[]? array, int cuantos)
        {
            var celdas = new List<CeldaAnalisis>();
            if (array != null)
            {
                for (int i = 1; i < cuantos && i < array.Length; i++)
                {
                    celdas.Add(new CeldaAnalisis(array[i].ToString()));
                }
            }
            return new FilaMatrizAnalisis(etiqueta, celdas);
        }

        // Valoración: 4 columnas (Global/Unos/Equis/Doses); sólo se muestran las entradas con
        // valor != 0, empezando en NoValoresValoracion*[0] (legacy: CtrlAnalisisValoraciones).
        private static SeccionAnalisisItem SeccionValoracion(string clave, string titulo, ContenedorAnalisis? c)
        {
            var s = new SeccionAnalisisItem(clave, titulo) { Tipo = TipoSeccionAnalisis.Valoracion };
            if (c == null || !c.UsaValoraciones)
            {
                s.Tipo = TipoSeccionAnalisis.Vacia;
                s.Detalle = "Valoración: No hay datos para analizar";
                return s;
            }
            string tipo = c.TipoValoracion switch
            {
                "suma" => "Por Sumas",
                "multiplo" => "Por Productos",
                _ => c.TipoValoracion,
            };
            s.Detalle = "Valoración: distribución de columnas por valor (" + tipo + ").";

            s.ColumnasValoracion.Add(ColumnaValoracionDesde("Global", c.ValoracionesGlobales, c.NoValoresValoracionGlobal));
            s.ColumnasValoracion.Add(ColumnaValoracionDesde("Unos", c.ValoracionesUnos, c.NoValoresValoracionUnos));
            s.ColumnasValoracion.Add(ColumnaValoracionDesde("Equis", c.ValoracionesEquis, c.NoValoresValoracionEquis));
            s.ColumnasValoracion.Add(ColumnaValoracionDesde("Doses", c.ValoracionesDoses, c.NoValoresValoracionDoses));
            return s;
        }

        private static ColumnaValoracion ColumnaValoracionDesde(string titulo, List<int>? valores, List<int>? noValores)
        {
            var entradas = new List<EntradaValorAnalisis>();
            if (valores != null)
            {
                // Legacy: i parte de NoValoresValoracion*[0]; sólo se añaden los != 0.
                int inicio = (noValores != null && noValores.Count > 0) ? noValores[0] : 0;
                if (inicio < 0) inicio = 0;
                for (int i = inicio; i < valores.Count; i++)
                {
                    if (valores[i] != 0)
                    {
                        entradas.Add(new EntradaValorAnalisis(i.ToString(), valores[i].ToString()));
                    }
                }
            }
            return new ColumnaValoracion(titulo, entradas);
        }

        // Columnas probables: 3 matrices (Aciertos / Aciertos seguidos / Fallos seguidos).
        // Cada matriz: cabecera Nº + 0..NumeroPartidos, 1 fila por CP. Legacy: CtrlDataGridViewCPs.
        private static SeccionAnalisisItem SeccionCPs(string clave, string titulo, ContenedorAnalisis? c)
        {
            var s = new SeccionAnalisisItem(clave, titulo) { Tipo = TipoSeccionAnalisis.Matrices };
            if (c == null || !c.UsaCPs)
            {
                s.Tipo = TipoSeccionAnalisis.Vacia;
                s.Detalle = "Columnas probables: No hay datos para analizar";
                return s;
            }
            var cps = c.ContColumnasProbables;
            s.Detalle = $"Columnas probables: {cps.Count} columnas (aciertos / aciertos seguidos / fallos seguidos).";

            s.Bloques.Add(new BloqueMatrizAnalisis("Aciertos", MatrizCPs(cps, ColumnaCP.Aciertos)));
            s.Bloques.Add(new BloqueMatrizAnalisis("Aciertos seguidos", MatrizCPs(cps, ColumnaCP.AciertosSeguidos)));
            s.Bloques.Add(new BloqueMatrizAnalisis("Fallos seguidos", MatrizCPs(cps, ColumnaCP.FallosSeguidos)));
            return s;
        }

        private enum ColumnaCP { Aciertos, AciertosSeguidos, FallosSeguidos }

        private static MatrizAnalisis MatrizCPs(List<ContenedorColumnasProbables> cps, ColumnaCP cual)
        {
            int columnas = VariablesGlobales.NumeroPartidos; // legacy: j de 0 a NumeroPartidos inclusive
            var filas = new List<FilaMatrizAnalisis>(cps.Count + 1);

            // Cabecera: índices 0..NumeroPartidos.
            var cab = new List<CeldaAnalisis>(columnas + 1);
            for (int j = 0; j <= columnas; j++)
            {
                cab.Add(new CeldaAnalisis(j.ToString(), esCabecera: true));
            }
            filas.Add(new FilaMatrizAnalisis("Nº", cab, esCabecera: true));

            for (int i = 0; i < cps.Count; i++)
            {
                int[] datos = cual switch
                {
                    ColumnaCP.Aciertos => cps[i].NoAC,
                    ColumnaCP.AciertosSeguidos => cps[i].NoACS,
                    _ => cps[i].NoFS,
                };
                var celdas = new List<CeldaAnalisis>(columnas + 1);
                for (int j = 0; j <= columnas; j++)
                {
                    string valor = (datos != null && j < datos.Length) ? datos[j].ToString() : "0";
                    celdas.Add(new CeldaAnalisis(valor));
                }
                filas.Add(new FilaMatrizAnalisis("CP " + (i + 1), celdas));
            }
            return new MatrizAnalisis(filas);
        }

        // Formatos: un bloque por grupo. Cada grupo lleva una matriz "Líneas" (índice -> nº columnas)
        // y otra "Globales" (clave -> nº columnas). Legacy: CtrlAnalisisFormatosSignos.
        private static SeccionAnalisisItem SeccionFormatos(string clave, string titulo, ContenedorAnalisis? c)
        {
            var s = new SeccionAnalisisItem(clave, titulo) { Tipo = TipoSeccionAnalisis.Matrices };
            if (c == null || !c.UsaFormatos)
            {
                s.Tipo = TipoSeccionAnalisis.Vacia;
                s.Detalle = "Formatos: No hay datos para analizar";
                return s;
            }
            var grupos = c.ContenedorFormatosSignos;
            s.Detalle = $"Formatos: {grupos.Count} grupos de formatos.";
            for (int i = 0; i < grupos.Count; i++)
            {
                ContenedorFormatos grupo = grupos[i];

                // Líneas: índice -> nº columnas (legacy: LlenarNumerosLineas).
                int[]? lineas = grupo.AciertosFormatosSignos;
                if (lineas != null)
                {
                    var cab = new List<CeldaAnalisis>(lineas.Length);
                    var val = new List<CeldaAnalisis>(lineas.Length);
                    for (int j = 0; j < lineas.Length; j++)
                    {
                        cab.Add(new CeldaAnalisis(j.ToString(), esCabecera: true));
                        val.Add(new CeldaAnalisis(lineas[j].ToString()));
                    }
                    var filasLineas = new List<FilaMatrizAnalisis>
                    {
                        new FilaMatrizAnalisis("Líneas", cab, esCabecera: true),
                        new FilaMatrizAnalisis("Columnas", val),
                    };
                    s.Bloques.Add(new BloqueMatrizAnalisis(
                        "Grupo " + (i + 1) + " — Líneas", new MatrizAnalisis(filasLineas)));
                }

                // Globales: clave -> nº columnas (legacy: LlenarNumerosGlobales; índice de fila = posición).
                var globales = grupo.AciertosGlobalesFormatos;
                if (globales != null && globales.Count > 0)
                {
                    var cabG = new List<CeldaAnalisis>(globales.Count);
                    var valG = new List<CeldaAnalisis>(globales.Count);
                    for (int j = 0; j < globales.Count; j++)
                    {
                        // Legacy muestra el índice de posición (i) en la cabecera y el valor de la clave.
                        cabG.Add(new CeldaAnalisis(j.ToString(), esCabecera: true));
                        valG.Add(new CeldaAnalisis(globales[globales.Keys[j]].ToString()));
                    }
                    var filasGlob = new List<FilaMatrizAnalisis>
                    {
                        new FilaMatrizAnalisis("Global", cabG, esCabecera: true),
                        new FilaMatrizAnalisis("Columnas", valG),
                    };
                    s.Bloques.Add(new BloqueMatrizAnalisis(
                        "Grupo " + (i + 1) + " — Globales", new MatrizAnalisis(filasGlob)));
                }
            }
            return s;
        }
    }
}
