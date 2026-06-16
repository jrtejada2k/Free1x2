using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Free1X2.Analisis;
using Free1X2.MotorCalculo;

namespace Free1X2.WinUI.Views.Ported
{
    // Port of legacy WinForms "VisorAnalisisColumnasFrm" (Free1X2.UI.VisorAnalisisColumnasFrm).
    // The legacy form is a read-only tabbed viewer: a TabControl whose visible tabs depend on
    // which "VariablesGlobales.Analizar*" flags are active. Each visible tab hosts a custom
    // analysis UserControl (CtrlAnalisisVX2, CtrlAnalisisSSeguidos, CtrlDibujos, etc.).
    //
    // Cableado al motor real: lee el handoff estático (UltimoContenedor = ContenedorAnalisisGlobal,
    // UltimoGrupo = Grupo) que deja el hook App.CablearHooksDominio (AnalisisUi.MostrarVisor) y
    // construye la lista de secciones activas a partir de VariablesGlobales.Analizar*, volcando
    // los datos numéricos de ContenedorAnalisis (matrices int[,], figuras y listas) en filas
    // legibles. Los UserControls de análisis del WinForms (CtrlAnalisis*) no están portados al
    // dominio: aquí se muestra la misma información subyacente en una rejilla en su lugar.

    /// <summary>Una fila de datos de una sección de análisis (etiqueta + serie de valores).</summary>
    public sealed class FilaAnalisis
    {
        public string Etiqueta { get; set; } = string.Empty;
        public string Valores { get; set; } = string.Empty;
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

        /// <summary>Filas de datos de la sección (vuelco de las matrices/figuras del ContenedorAnalisis).</summary>
        public ObservableCollection<FilaAnalisis> Filas { get; } = new();

        // Marcador textual del estado (string para TextBlock.Text; regla anti-crash: no bindear bool).
        public string EstadoMarcador => Activo ? "●" : string.Empty;

        partial void OnActivoChanged(bool value)
        {
            OnPropertyChanged(nameof(EstadoMarcador));
        }
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

        /// <summary>Filas de datos de la sección seleccionada (rejilla del detalle).</summary>
        public ObservableCollection<FilaAnalisis> FilasDetalle =>
            SeccionSeleccionada?.Filas ?? new ObservableCollection<FilaAnalisis>();

        partial void OnSeccionSeleccionadaChanged(SeccionAnalisisItem? value)
        {
            OnPropertyChanged(nameof(DetalleTitulo));
            OnPropertyChanged(nameof(DetalleTexto));
            OnPropertyChanged(nameof(FilasDetalle));
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

        [RelayCommand]
        private void AbrirSeccion()
        {
            // En el WinForms cada AñadirX() insertaba el UserControl de análisis (CtrlAnalisisVX2,
            // CtrlAnalisisSSeguidos, CtrlDibujos, ...). Esos controles NO están portados al dominio;
            // aquí los datos subyacentes ya se muestran en FilasDetalle al seleccionar la sección.
            // TODO[controles]: portar los CtrlAnalisis* (Free1X2/UI/Controls/Analisis/) para una
            //   presentación visual equivalente; ver VisorAnalisisColumnasFrm.AñadirVX2/... (líneas 208-369).
        }

        /// <summary>
        /// Reconstruye la lista de secciones activas (legacy: MostrarDatos -> AñadirX()/BorrarTab()
        /// según VariablesGlobales.Analizar*), volcando los datos del ContenedorAnalisis.
        /// </summary>
        private void ConstruirSecciones()
        {
            Secciones.Clear();
            ContenedorAnalisis? c = _contenedor?.AnalisisGrupos;

            if (VariablesGlobales.AnalizarVX2)
                Secciones.Add(SeccionMatriz("VX2", "VX2 — Variantes, Equis, Doses",
                    c?.VX2, new[] { "Variantes", "Equis", "Doses" }));

            if (VariablesGlobales.AnalizarSeguidos)
                Secciones.Add(SeccionMatriz("SignosSeguidos", "Signos Seguidos",
                    c?.Seguidos, new[] { "Variantes seguidas", "Unos seguidos", "Equis seguidas", "Doses seguidos" }));

            if (VariablesGlobales.AnalizarDibujos)
                Secciones.Add(SeccionMatriz("Dibujos", "Dibujos", c?.Dibujos, null));

            if (VariablesGlobales.AnalizarInterrupciones)
                Secciones.Add(SeccionMatriz("Interrupciones", "Interrupciones", c?.Interrupciones, new[]
                {
                    "Globales", "Variantes", "Unos", "Equis", "Doses",
                    "Globales seguidas", "Variantes seguidas", "Unos seguidos", "Equis seguidas", "Doses seguidos"
                }));

            if (VariablesGlobales.AnalizarContactos)
                Secciones.Add(SeccionMatriz("Contactos", "Contactos", c?.Contactos, new[]
                {
                    "1X", "12", "X2", "11", "XX", "22", "1V", "XV", "2V", "VV"
                }));

            if (VariablesGlobales.AnalizarPesos)
                Secciones.Add(SeccionMatriz("Pesos", "Pesos Numéricos", c?.Pesos, new[]
                {
                    "Global", "Variantes", "Unos", "Equis", "Doses"
                }));

            if (VariablesGlobales.AnalizarDistancias)
                Secciones.Add(SeccionMatriz("Distancias", "Distancias", c?.Distancias, new[]
                {
                    "Variantes", "Unos", "Equis", "Doses"
                }));

            if (VariablesGlobales.AnalizarControlGrupos)
                Secciones.Add(SeccionVector("ControlGrupos", "Control Grupos",
                    _contenedor?.ColumnasPorFallosDeGrupos));

            if (VariablesGlobales.AnalizarControlConjuntos)
                Secciones.Add(SeccionVector("ControlConjuntos", "Control Conjuntos",
                    _contenedor?.ColumnasPorFallosDeConjuntos));

            if (VariablesGlobales.AnalizarSimetrias)
                Secciones.Add(SeccionSimetrias("Simetrias", "Simetrías", c));

            if (VariablesGlobales.AnalizarSimetriasII)
                Secciones.Add(SeccionDiferencias("Diferencias", "Diferencias", c));

            if (VariablesGlobales.AnalizarValoracion)
                Secciones.Add(SeccionValoracion("Valoracion", "Valoración", c));

            if (VariablesGlobales.AnalizarCPs)
                Secciones.Add(SeccionCPs("ColumnasProbables", "Columnas Probables", c));

            if (VariablesGlobales.AnalizarFormatos)
                Secciones.Add(SeccionFormatos("Formatos", "Formatos", c));

            if (Secciones.Count == 0)
            {
                var vacia = new SeccionAnalisisItem("SinAnalisis", "Sin análisis activos")
                {
                    Detalle = _contenedor == null
                        ? "No se ha recibido ningún análisis. Ejecuta un análisis para ver los datos."
                        : "No hay ningún tipo de análisis activo (VariablesGlobales.Analizar*)."
                };
                Secciones.Add(vacia);
            }
        }

        // Vuelca una matriz int[fila, columna] en filas etiquetadas (una fila por concepto).
        private static SeccionAnalisisItem SeccionMatriz(string clave, string titulo, int[,]? matriz, string[]? etiquetas)
        {
            var s = new SeccionAnalisisItem(clave, titulo);
            if (matriz == null)
            {
                s.Detalle = titulo + ": no hay datos para analizar";
                return s;
            }
            int filas = matriz.GetLength(0);
            int cols = matriz.GetLength(1);
            s.Detalle = $"{titulo}: {filas} conceptos × {cols} valores (cada celda = nº de columnas).";
            for (int i = 0; i < filas; i++)
            {
                var sb = new StringBuilder();
                for (int j = 0; j < cols; j++)
                {
                    if (j > 0) sb.Append("  ");
                    sb.Append(j).Append(':').Append(matriz[i, j]);
                }
                string etiqueta = (etiquetas != null && i < etiquetas.Length) ? etiquetas[i] : "Fila " + i;
                s.Filas.Add(new FilaAnalisis { Etiqueta = etiqueta, Valores = sb.ToString() });
            }
            return s;
        }

        // Vuelca un vector int[] (columnas por nº de fallos) en filas indexadas.
        private static SeccionAnalisisItem SeccionVector(string clave, string titulo, int[]? vector)
        {
            var s = new SeccionAnalisisItem(clave, titulo);
            if (vector == null)
            {
                s.Detalle = titulo + ": no hay datos para analizar";
                return s;
            }
            s.Detalle = $"{titulo}: columnas por nº de fallos ({vector.Length} valores).";
            for (int i = 0; i < vector.Length; i++)
            {
                s.Filas.Add(new FilaAnalisis { Etiqueta = "Fallos " + i, Valores = vector[i].ToString() });
            }
            return s;
        }

        private static SeccionAnalisisItem SeccionSimetrias(string clave, string titulo, ContenedorAnalisis? c)
        {
            var s = new SeccionAnalisisItem(clave, titulo);
            var lista = c?.ContenedorSim;
            if (lista == null || lista.Count == 0)
            {
                s.Detalle = "Simetrías: No hay datos para analizar";
                return s;
            }
            s.Detalle = $"Simetrías: {lista.Count} grupos de aciertos.";
            for (int i = 0; i < lista.Count; i++)
            {
                ContenedorSimetrias cs = lista[i];
                s.Filas.Add(new FilaAnalisis { Etiqueta = "Aciertos " + cs.Aciertos, Valores = cs.Columnas.ToString() });
            }
            return s;
        }

        private static SeccionAnalisisItem SeccionDiferencias(string clave, string titulo, ContenedorAnalisis? c)
        {
            var s = new SeccionAnalisisItem(clave, titulo);
            var lista = c?.ContenedorDiferencias;
            if (lista == null || lista.Count == 0)
            {
                s.Detalle = "Diferencias: No hay datos para analizar";
                return s;
            }
            s.Detalle = $"Diferencias: {lista.Count} contenedores.";
            for (int i = 0; i < lista.Count; i++)
            {
                s.Filas.Add(new FilaAnalisis { Etiqueta = "Diferencia " + (i + 1), Valores = SerieDeArray(lista[i].Variantes) });
            }
            return s;
        }

        private static SeccionAnalisisItem SeccionValoracion(string clave, string titulo, ContenedorAnalisis? c)
        {
            var s = new SeccionAnalisisItem(clave, titulo);
            if (c == null || !c.UsaValoraciones)
            {
                s.Detalle = "Valoración: No hay datos para analizar";
                return s;
            }
            s.Detalle = "Valoración: distribución de columnas por valor (" + c.TipoValoracion + ").";
            s.Filas.Add(new FilaAnalisis { Etiqueta = "Global", Valores = SerieDeLista(c.ValoracionesGlobales) });
            s.Filas.Add(new FilaAnalisis { Etiqueta = "Unos", Valores = SerieDeLista(c.ValoracionesUnos) });
            s.Filas.Add(new FilaAnalisis { Etiqueta = "Equis", Valores = SerieDeLista(c.ValoracionesEquis) });
            s.Filas.Add(new FilaAnalisis { Etiqueta = "Doses", Valores = SerieDeLista(c.ValoracionesDoses) });
            return s;
        }

        private static SeccionAnalisisItem SeccionCPs(string clave, string titulo, ContenedorAnalisis? c)
        {
            var s = new SeccionAnalisisItem(clave, titulo);
            if (c == null || !c.UsaCPs)
            {
                s.Detalle = "Columnas probables: No hay datos para analizar";
                return s;
            }
            var cps = c.ContColumnasProbables;
            s.Detalle = $"Columnas probables: {cps.Count} columnas.";
            for (int i = 0; i < cps.Count; i++)
            {
                s.Filas.Add(new FilaAnalisis { Etiqueta = "CP " + (i + 1), Valores = SerieDeArray(cps[i].NoAC) });
            }
            return s;
        }

        private static SeccionAnalisisItem SeccionFormatos(string clave, string titulo, ContenedorAnalisis? c)
        {
            var s = new SeccionAnalisisItem(clave, titulo);
            if (c == null || !c.UsaFormatos)
            {
                s.Detalle = "Formatos: No hay datos para analizar";
                return s;
            }
            var grupos = c.ContenedorFormatosSignos;
            s.Detalle = $"Formatos: {grupos.Count} grupos de formatos.";
            for (int i = 0; i < grupos.Count; i++)
            {
                s.Filas.Add(new FilaAnalisis
                {
                    Etiqueta = "Grupo " + (i + 1),
                    Valores = SerieDeArray(grupos[i].AciertosFormatosSignos)
                });
            }
            return s;
        }

        private static string SerieDeArray(int[]? array)
        {
            if (array == null || array.Length == 0) return "(sin datos)";
            var sb = new StringBuilder();
            for (int i = 0; i < array.Length; i++)
            {
                if (i > 0) sb.Append("  ");
                sb.Append(i).Append(':').Append(array[i]);
            }
            return sb.ToString();
        }

        private static string SerieDeLista(List<int>? lista)
        {
            if (lista == null || lista.Count == 0) return "(sin datos)";
            var sb = new StringBuilder();
            for (int i = 0; i < lista.Count; i++)
            {
                if (i > 0) sb.Append("  ");
                sb.Append(i).Append(':').Append(lista[i]);
            }
            return sb.ToString();
        }
    }
}
