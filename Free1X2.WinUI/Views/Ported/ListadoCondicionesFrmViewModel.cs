using System.Collections.ObjectModel;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

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
    }

    /// <summary>
    /// ViewModel para ListadoCondicionesFrmPage.
    /// Replica el propósito del legacy Free1X2.UI.ListadoCondicionesFrm:
    /// mostrar en árbol jerárquico todas las condiciones/filtros configurados
    /// (Pronóstico Base, Filtro de Columnas, Grupos y sus filtros, Control de
    /// grupos, IfThen) y permitir expandir/colapsar y exportar a texto/HTML.
    /// </summary>
    public partial class ListadoCondicionesFrmViewModel : ObservableObject
    {
        // Raíz del árbol de condiciones (en el legacy: nodoPrincipal "Listado de Condiciones").
        public ObservableCollection<CondicionNodo> Condiciones { get; } = new();

        // Datos de entrada que el legacy recibía por propiedades públicas
        // (Equipos, Pronosticos, ArchivoFiltro, GrupoDePartidos,
        //  ControladorDeGrupos, ControladorDeIfThen).
        // TODO[dominio]: inyectar estos modelos de dominio
        // (Free1X2.MotorCalculo.GrupoPartidos / ControladorGrupos / ControladorIfThen)
        // cuando se porte el motor. Mantener como TODO según reglas de aislamiento.

        public ListadoCondicionesFrmViewModel()
        {
            // TODO[dominio]: reconstruir el árbol como en
            // ListadoCondicionesFrm.ListadoCondicionesFrm_Load():
            //   - Nodo "Pronóstico Base" con un nodo por partido (Pronosticos[]).
            //   - Nodo "Filtro de Columnas" (ArchivoFiltro o "No se usa ningún Filtro").
            //   - Nodo "Grupos": por cada Grupo de GrupoDePartidos.CtrlGrupos.GruposPartidos,
            //     sus partidos activos, filtro parcial y condiciones (NoVariantes,
            //     SignosSeguidos, Dibujos, ColProbables, NoInterrupciones, PesosNumericos,
            //     ValoracionSignos, Distancias, GruposEquipos, Contactos, FormatosSignos,
            //     Formatos123, Simetrias, Diferencias).
            //   - Nodo "Control de grupos" (ControladorDeGrupos.ControlesGrupos / ControlesConjuntos).
            //   - Nodo "IfThen" (ControladorDeIfThen, activo/inactivo y sus condiciones/grupos).
            // No implementar aquí: depende del motor de dominio aún no portado.

            // Placeholder visible mientras no exista el motor portado.
            Condiciones.Add(new CondicionNodo("Listado de Condiciones")
            {
                IsExpanded = true
            });
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
        private void ExportarTexto()
        {
            // TODO[dominio]: equivalente de ListadoCondicionesFrm.Exportar_Click():
            //   - Generar texto plano con ExportarATexto(treeVwCondiciones.Nodes[0], 0).
            //   - Mostrar un FileSavePicker (.txt), escribir UTF-8 y abrir el archivo.
            // El SaveFileDialog/StreamWriter/Process.Start de WinForms se reemplaza
            // por Windows.Storage.Pickers.FileSavePicker + StorageFile en WinUI.
        }

        [RelayCommand]
        private void ExportarHtml()
        {
            // TODO[dominio]: equivalente de ListadoCondicionesFrm.ExportarHtml_Click():
            //   - Generar HTML con ExportarAHtml(treeVwCondiciones.Nodes[0], 0),
            //     envolviendo en <html><head><title>Listado de Condiciones</title>...</body>.
            //   - Mostrar un FileSavePicker (.html), escribir UTF-8 y abrir el archivo.
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
