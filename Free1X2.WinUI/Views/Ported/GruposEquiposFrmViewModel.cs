using System.Collections.Generic;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// Un partido de la quiniela dentro del editor de Grupos de Equipos.
/// Cada partido ofrece dos selecciones independientes (equipo de casa "1" y
/// equipo de fuera "2"); marcar ambas equivale al signo "X" (valor '3' en el
/// modelo legacy <c>GrupoEquipos.Pronosticos</c>).
/// Equivale a los pares de Label "lblCasaN"/"lblFueraN" del WinForms
/// <c>GruposEquiposFrm.AñadirCasillas()</c>.
/// </summary>
public partial class PartidoEquipos : ObservableObject
{
    public int Numero { get; init; }

    /// <summary>Texto "N" mostrado en la columna de número (TextBlock exige string).</summary>
    public string NumeroTexto => Numero.ToString();

    [ObservableProperty]
    private string _equipoCasa = "";

    [ObservableProperty]
    private string _equipoFuera = "";

    // Selección del equipo de casa (Label "lblCasaN" resaltado en el WinForms).
    [ObservableProperty]
    private bool _casaSeleccionado;

    // Selección del equipo de fuera (Label "lblFueraN" resaltado en el WinForms).
    [ObservableProperty]
    private bool _fueraSeleccionado;
}

/// <summary>
/// ViewModel para la pantalla "Grupos de Equipos".
/// Replica las entradas del WinForms <c>GruposEquiposFrm</c>, que define
/// agrupaciones de equipos para el filtro <c>FiltroGruposEquipos</c>:
///
/// - Pestaña "Grupos Equipos": una rejilla de 14 partidos donde se marcan los
///   equipos elegidos (casa/fuera) y se fijan Victorias / Empates / Derrotas /
///   Suma de Puntos esperados para ese grupo. Se navega entre varios grupos
///   (anterior / siguiente / eliminar).
/// - Pestaña "Relaciones": condiciones que relacionan grupos por su índice
///   ("Grupos Equipos") y suman Victorias / Empates / Derrotas / Puntos.
///   Se navega entre varias relaciones.
///
/// La lógica de cálculo, validación numérica y persistencia queda como TODO
/// citando las clases legacy; aquí solo se modela el estado de la UI.
/// </summary>
public partial class GruposEquiposFrmViewModel : ObservableObject
{
    public GruposEquiposFrmViewModel()
    {
        // TODO(dominio): rellenar nombres reales de equipos desde
        //   FormPadre.pronosticos.BuscarControl(n).EquipoCasa / EquipoFuera
        //   (WinForms GruposEquiposFrm.LlenaEquipos / getEquipo).
        for (int i = 1; i <= NumeroPartidos; i++)
        {
            Partidos.Add(new PartidoEquipos
            {
                Numero = i,
                EquipoCasa = "Equipo casa " + i,
                EquipoFuera = "Equipo fuera " + i,
            });
        }
    }

    // VariablesGlobales.NumeroPartidos del proyecto legacy (quiniela de 14 + P15).
    private const int NumeroPartidos = 14;

    /// <summary>
    /// Los 14 partidos con sus dos casillas de selección.
    /// Equivale a las casillas creadas en <c>AñadirCasillas()</c>.
    /// </summary>
    public ObservableCollection<PartidoEquipos> Partidos { get; } = new();

    // ===== Pestaña "Grupos Equipos": datos del grupo actual =====
    // Cadenas para permitir vacío y replicar SonTodosNumeros() del legacy.
    [ObservableProperty]
    private string _victorias = "";

    [ObservableProperty]
    private string _empates = "";

    [ObservableProperty]
    private string _derrotas = "";

    [ObservableProperty]
    private string _sumaPuntos = "";

    // Navegación de grupos (lblNoGruposEq "n/total" + btnPrev/btnNext).
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(GrupoActualTexto))]
    private int _grupoActual = 1;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(GrupoActualTexto))]
    private int _totalGrupos = 1;

    /// <summary>Proyección "n/total" para el TextBlock (x:Bind exige string).</summary>
    public string GrupoActualTexto => GrupoActual + "/" + TotalGrupos;

    // ===== Pestaña "Relaciones": datos de la relación actual =====
    [ObservableProperty]
    private string _gruposEquiposRel = "";

    [ObservableProperty]
    private string _sumaVictoriasRel = "";

    [ObservableProperty]
    private string _sumaEmpatesRel = "";

    [ObservableProperty]
    private string _sumaDerrotasRel = "";

    [ObservableProperty]
    private string _sumaPuntosRel = "";

    // Navegación de relaciones (lblNoRel1 "n/total" + btnPrevRel/btnNexRel).
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(RelacionActualTexto))]
    private int _relacionActual = 1;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(RelacionActualTexto))]
    private int _totalRelaciones = 1;

    /// <summary>Proyección "n/total" para el TextBlock (x:Bind exige string).</summary>
    public string RelacionActualTexto => RelacionActual + "/" + TotalRelaciones;

    // Habilita/deshabilita "anterior" cuando estamos en el primer elemento.
    public bool PuedeRetrocederGrupo => GrupoActual > 1;
    public bool PuedeRetrocederRelacion => RelacionActual > 1;

    // ===== Estado =====
    [ObservableProperty]
    private string _estado = "Preparado";

    // ----- Navegación de grupos -----

    [RelayCommand]
    private void GrupoAnterior()
    {
        // TODO(dominio): GruposEquiposFrm.btnPrev_Click -> CambiaGESelecionado(noGEPantalla-1):
        //   guardar el grupo actual y cargar el anterior desde arrayGE (List<GrupoEquipos>).
        if (GrupoActual > 1)
        {
            GrupoActual--;
            OnPropertyChanged(nameof(PuedeRetrocederGrupo));
        }
    }

    [RelayCommand]
    private void GrupoSiguiente()
    {
        // TODO(dominio): GruposEquiposFrm.btnNext_Click -> si TieneGEDatos() CambiaGESelecionado(noGEPantalla+1):
        //   crea un GrupoEquipos nuevo si no existe y avanza.
        GrupoActual++;
        if (GrupoActual > TotalGrupos)
        {
            TotalGrupos = GrupoActual;
        }
        OnPropertyChanged(nameof(PuedeRetrocederGrupo));
    }

    [RelayCommand]
    private void EliminarGrupo()
    {
        // TODO(dominio): GruposEquiposFrm.btnEliminarGrupo_Click -> BorrarGE(noGEPantalla)
        //   sobre arrayGE; reajustar índice y recargar pantalla.
        Estado = "Eliminar grupo (pendiente de portar dominio)";
    }

    // ----- Navegación de relaciones -----

    [RelayCommand]
    private void RelacionAnterior()
    {
        // TODO(dominio): GruposEquiposFrm.btnPrevRel_Click -> CambiaRelGE1Selecionado(relGE1Pantalla-1).
        if (RelacionActual > 1)
        {
            RelacionActual--;
            OnPropertyChanged(nameof(PuedeRetrocederRelacion));
        }
    }

    [RelayCommand]
    private void RelacionSiguiente()
    {
        // TODO(dominio): GruposEquiposFrm.btnNexRel_Click -> si TieneRelacion1Datos() CambiaRelGE1Selecionado(relGE1Pantalla+1).
        RelacionActual++;
        if (RelacionActual > TotalRelaciones)
        {
            TotalRelaciones = RelacionActual;
        }
        OnPropertyChanged(nameof(PuedeRetrocederRelacion));
    }

    [RelayCommand]
    private void EliminarRelacion()
    {
        // TODO(dominio): GruposEquiposFrm.btnEliminaGERel_Click -> BorrarRel1(relGE1Pantalla)
        //   sobre arrayRelaciones1 (List<RelacionGE1>); reajustar índice.
        Estado = "Eliminar relación (pendiente de portar dominio)";
    }

    // ----- Barra de comandos (menuCondiciones del WinForms) -----

    [RelayCommand]
    private void Aceptar()
    {
        // TODO(dominio): GruposEquiposFrm.menuCondiciones1_BOk -> GuardarDatos() + GuardarDatosRelacionesGE1()
        //   + analizador.GruposPartidos[...].ActivaFiltro(filtroGE) y cerrar la ventana.
        Estado = "Aceptar (pendiente de portar dominio)";
    }

    [RelayCommand]
    private void Guardar()
    {
        // TODO(dominio): GruposEquiposFrm.menuCondiciones1_BGuardar -> SaveFileDialog (*.geq/*.xml)
        //   + ArchivoCondiciones.GuardaArchivo(filtroGE). Usar FileSavePicker en el code-behind.
        Estado = "Guardar (pendiente de portar dominio)";
    }

    [RelayCommand]
    private void Abrir()
    {
        // TODO(dominio): GruposEquiposFrm.menuCondiciones1_BAbrir -> OpenFileDialog (*.geq/*.xml)
        //   + ArchivoCondiciones.AbrirArchivoCombinacion/LeeCondicion. Usar FileOpenPicker.
        Estado = "Abrir (pendiente de portar dominio)";
    }

    [RelayCommand]
    private void Borrar()
    {
        // TODO(dominio): GruposEquiposFrm.menuCondiciones1_BBorrar -> reinicia filtroGE = new FiltroGruposEquipos()
        //   tras confirmación.
        Estado = "Borrar (pendiente de portar dominio)";
    }

    [RelayCommand]
    private void Copiar()
    {
        // TODO(dominio): GruposEquiposFrm.menuCondiciones1_BCopiar -> guarda un .geq temporal en Temp/
        //   y habilita Pegar.
        Estado = "Copiar (pendiente de portar dominio)";
    }

    [RelayCommand]
    private void Pegar()
    {
        // TODO(dominio): GruposEquiposFrm.menuCondiciones1_BPegar -> abre el .geq temporal de Temp/.
        Estado = "Pegar (pendiente de portar dominio)";
    }

    [RelayCommand]
    private void Estadisticas()
    {
        // TODO(dominio): GruposEquiposFrm.menuCondiciones1_BEstadisticas -> ObtenerFiltroTemporal()
        //   + CalculadorEstadisticas.EstadisticasFiltro(...) y abrir VisorEstadisticas.
        Estado = "Estadísticas (pendiente de portar dominio)";
    }

    [RelayCommand]
    private void Cancelar()
    {
        // TODO(dominio): GruposEquiposFrm.menuCondiciones1_BCancelar -> cerrar la ventana sin guardar.
        Estado = "Cancelar (pendiente de portar dominio)";
    }
}
