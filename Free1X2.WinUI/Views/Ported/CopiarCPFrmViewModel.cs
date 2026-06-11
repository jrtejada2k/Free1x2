using System.Collections.Generic;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// ViewModel para la pantalla "Copiar Columnas".
/// Replica los campos de entrada del WinForms <c>CopiarCPFrm</c>:
/// un texto con los índices de columnas probables a copiar (txtCP) y una lista
/// multi-selección de grupos destino (cmbGrupo, un ListBox MultiExtended).
/// El usuario indica qué columnas (por índice, lista "1,3,5" o rango "1-5") quiere
/// copiar y a qué grupo(s) del boleto deben copiarse.
/// </summary>
public partial class CopiarCPFrmViewModel : ObservableObject
{
    /// <summary>
    /// Grupos destino disponibles (equivalen a los Items del ListBox cmbGrupo del
    /// WinForms, que se llenaba en InicializarGruposDropDown con "Boleto Base" + los
    /// índices de MotorCalculo.GruposPartidos). Se exponen como colección para un
    /// ListView en modo selección múltiple.
    /// </summary>
    public ObservableCollection<string> Grupos { get; } = new();

    /// <summary>
    /// Grupos seleccionados como destino de la copia (equivale a las filas
    /// seleccionadas del ListBox MultiExtended cmbGrupo).
    /// </summary>
    public ObservableCollection<string> GruposSeleccionados { get; } = new();

    // ===== Texto con los índices de columnas a copiar (txtCP) =====
    // Acepta un índice ("5"), una lista ("1,3,5") o un rango ("1-5"),
    // tal como interpretaba EncuentraSeparador/ObtenCP en el WinForms.
    [ObservableProperty]
    private string _columnas = "";

    // ===== Estado / feedback =====
    [ObservableProperty]
    private string _estado = "Indica las columnas y el grupo destino.";

    public CopiarCPFrmViewModel()
    {
        // TODO(dominio): poblar Grupos a partir de
        //   MainForm.MotorCalculo.GruposPartidos, replicando
        //   CopiarCPFrm.InicializarGruposDropDown():
        //     - el índice 0 se muestra como "Boleto Base";
        //     - el resto como su número de grupo (1, 2, ...).
        //   También preseleccionar el grupo en pantalla (MainForm.NoGrupoPantalla).
        // Datos de ejemplo para visualizar la pantalla mientras se porta el dominio:
        Grupos.Add("Boleto Base");
    }

    /// <summary>
    /// Equivale a <c>btnCopiar_Click</c> -> <c>CopiarColumnas()</c> del WinForms:
    /// valida los índices y copia cada ColumnaProbable a los grupos seleccionados.
    /// </summary>
    [RelayCommand]
    private void Copiar()
    {
        if (string.IsNullOrWhiteSpace(Columnas))
        {
            Estado = "Introduce al menos una columna a copiar.";
            return;
        }

        if (GruposSeleccionados.Count == 0)
        {
            Estado = "Selecciona al menos un grupo destino.";
            return;
        }

        // TODO(dominio): portar CopiarCPFrm.CopiarColumnas().
        //   - Parsear Columnas con la lógica de ObtenCP/EncuentraSeparador:
        //       "," -> lista de índices; "-" -> rango [min..max]; si no, índice único.
        //   - Validar con IndicesValidos contra el tamaño de grupoCP.
        //   - Para cada grupo seleccionado (ObtenGrupo), obtener su
        //     FiltroColProbables (Filtro.ColProbables) y, por cada índice, clonar la
        //     ColumnaProbable (Pronosticos, aciertos/seguidos/fallos y, si
        //     ToleranciaLocalActiva, las tolerancias) y añadirla:
        //       * si el grupo es el de pantalla -> grupoCP.Add(copia);
        //       * en otro caso -> filtroCP.ColProbables.Add(copia),
        //         ContieneDatos = true, IsActive = true.
        //   - Llamar a parentForm.CambiaCPSelecionado() y cerrar (Close()).

        Estado = $"Copiar \"{Columnas}\" a {GruposSeleccionados.Count} grupo(s) (pendiente de portar dominio).";
    }

    /// <summary>
    /// Equivale a <c>btnCrearGrupos_Click</c> del WinForms: abre un diálogo para
    /// crear N grupos nuevos y los añade a la lista de destinos.
    /// </summary>
    [RelayCommand]
    private void CrearGrupos()
    {
        // TODO(dominio): portar CopiarCPFrm.btnCrearGrupos_Click().
        //   El WinForms abría CrearGruposFrm (ShowDialog), leía udNumGrupos.Value y,
        //   por cada grupo nuevo, creaba un Grupo con partidos 1..14 activos
        //   (PonerPartidosActivos("1,2,...,14")), lo añadía a
        //   MainForm.analizador.GruposPartidos, llamaba a ActualizarGruposPronostico()
        //   y agregaba el índice del nuevo grupo a cmbGrupo.Items.
        //   En WinUI: lanzar la página/diálogo de creación de grupos y, al volver,
        //   refrescar la colección Grupos.

        Estado = "Crear grupos (pendiente de portar CrearGruposFrm).";
    }

    /// <summary>
    /// Equivale a <c>btnCancelar_Click</c> del WinForms (Close()).
    /// </summary>
    [RelayCommand]
    private void Cancelar()
    {
        // TODO(dominio/navegación): cerrar/navegar atrás. En el WinForms era Close().
        Estado = "Cancelado.";
    }
}
