// Free1X2 · WinUI 3 — WIN3
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Free1X2.MotorCalculo;
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
            var lineaTexto = new ControladorIfThen(); // sólo se usa getLineaTexto(IFiltro) (resumen de cada filtro)
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
                    // getLineaTexto da el resumen canónico de cada filtro (Cantidad/Dibujo/Distancia/...).
                    string resumen = lineaTexto.getLineaTexto(filtro);
                    if (!string.IsNullOrEmpty(resumen))
                    {
                        nodoFiltroCond.AgregarHijo(resumen);
                    }
                    // TODO[detalle]: desglose completo por tipo de filtro (CP con Relaciones I/II/III y
                    //   Control de Fallos, GruposEquipos, Valoraciones, Formatos, Simetrías, Diferencias...)
                    //   tal como Free1X2/UI/ListadoCondicionesFrm.cs líneas 143-679 (switch por NombreFiltro).
                    //   getLineaTexto() ya cubre el resumen de una línea; el árbol detallado requiere
                    //   exponer/portar los getters de esos filtros y queda fuera del cableado del motor.
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
