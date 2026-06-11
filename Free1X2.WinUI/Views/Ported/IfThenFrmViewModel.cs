using System.Collections.Generic;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// Una relación "si ... entonces ..." ya añadida a una de las listas del filtro.
/// Equivale a un ListViewItem (texto + subitem) de listaCondiciones / listaGrupos del IfThenFrm legacy.
/// </summary>
public partial class RelacionIfThenViewModel : ObservableObject
{
    public RelacionIfThenViewModel(string si, string entonces)
    {
        Si = si;
        Entonces = entonces;
    }

    /// <summary>Parte "Si se produce ..." de la relación.</summary>
    public string Si { get; }

    /// <summary>Parte "... debe cumplirse que ..." de la relación.</summary>
    public string Entonces { get; }

    /// <summary>Marca de selección (la columna de checkboxes del ListView legacy, usada para borrar).</summary>
    [ObservableProperty]
    private bool _seleccionada;
}

/// <summary>
/// ViewModel de la pantalla "Condiciones relacionadas (if-then)".
///
/// Replica las dos pestañas del IfThenFrm WinForms:
///  - Condiciones sencillas: relaciona una condición de filtro (Si) con otra (Entonces).
///  - Grupos: relaciona un grupo de partidos (Si) con otro grupo (Entonces).
///
/// Solo gestiona el ESTADO de la interfaz. El cálculo, la persistencia y la apertura de
/// otros formularios se delegan al dominio legacy (ver TODOs en los comandos).
/// </summary>
public partial class IfThenFrmViewModel : ObservableObject
{
    // Lista de condiciones genéricas (comboCG_If / comboCG_Then del form legacy, método llenarCombo()).
    private static readonly IReadOnlyList<string> CondicionesGenericas = new[]
    {
        "Cantidad de signos",
        "Dibujos",
        "Signos Seguidos",
        "Interrupciones",
        "Pesos numéricos",
        "Distancias",
        "Contactos",
    };

    public IfThenFrmViewModel()
    {
        Relaciones = new ObservableCollection<RelacionIfThenViewModel>();
        RelacionesGrupos = new ObservableCollection<RelacionIfThenViewModel>();

        // TODO(dominio): cargar datos previos desde analizador.IfThen (ControladorIfThen)
        //   y rellenar GruposDisponibles con analizador.GruposPartidos.
        //   Legacy IfThenFrm.cargarDatos(analizador.IfThen) + llenarGrupos().
        GruposDisponibles = new ReadOnlyCollection<string>(new List<string>());
    }

    // ======================= Pestaña 1: condiciones sencillas =======================

    /// <summary>Opciones del desplegable "Condición genérica" (mismas para Si y Entonces).</summary>
    public IReadOnlyList<string> CondicionesGenericasDisponibles => CondicionesGenericas;

    // ---- Bloque "Si ..." ----

    [ObservableProperty]
    private string? _condicionGenericaIf;

    /// <summary>
    /// Opciones de "Condición específica" del bloque Si. Se rellenarían al cambiar la genérica.
    /// </summary>
    [ObservableProperty]
    private IReadOnlyList<string> _condicionesEspecificasIf = new List<string>();

    [ObservableProperty]
    private string? _condicionEspecificaIf;

    /// <summary>Valor numérico del bloque Si (upDown_If; 0..NumeroPartidos según la condición).</summary>
    [ObservableProperty]
    private double _valorIf;

    /// <summary>Negación de la condición Si (chkNoCond_If, prefijo "(NO) ").</summary>
    [ObservableProperty]
    private bool _negadoIf;

    // ---- Bloque "Entonces ..." ----

    [ObservableProperty]
    private string? _condicionGenericaThen;

    [ObservableProperty]
    private IReadOnlyList<string> _condicionesEspecificasThen = new List<string>();

    [ObservableProperty]
    private string? _condicionEspecificaThen;

    /// <summary>Valor numérico del bloque Entonces (upDown_Then).</summary>
    [ObservableProperty]
    private double _valorThen;

    /// <summary>Negación de la condición Entonces (chkNoCond_Then).</summary>
    [ObservableProperty]
    private bool _negadoThen;

    /// <summary>Relaciones añadidas en la pestaña de condiciones sencillas (listaCondiciones).</summary>
    public ObservableCollection<RelacionIfThenViewModel> Relaciones { get; }

    /// <summary>
    /// Rango de aciertos: cuántas de las condiciones de la lista deben cumplirse
    /// (txtConds del form legacy). Cadena para admitir rangos del tipo "1-3".
    /// </summary>
    [ObservableProperty]
    private string _rangoCondiciones = string.Empty;

    /// <summary>Texto del contador "Condiciones en lista" (txtCondsLista). Solo lectura en la UI.</summary>
    public string CondicionesEnListaTexto => Relaciones.Count.ToString();

    // ======================= Pestaña 2: grupos =======================

    /// <summary>
    /// Grupos de partidos disponibles para relacionar (cmbGrupo_If / cmbGrupo_Then).
    /// Se rellenaría con analizador.GruposPartidos. Si solo hay un grupo, la pestaña no aplica.
    /// </summary>
    [ObservableProperty]
    private IReadOnlyList<string> _gruposDisponibles;

    [ObservableProperty]
    private string? _grupoIf;

    [ObservableProperty]
    private bool _negadoGrupoIf;

    [ObservableProperty]
    private string? _grupoThen;

    [ObservableProperty]
    private bool _negadoGrupoThen;

    /// <summary>Relaciones de grupos añadidas (listaGrupos).</summary>
    public ObservableCollection<RelacionIfThenViewModel> RelacionesGrupos { get; }

    /// <summary>Rango de aciertos de grupos (txtGrupos del form legacy).</summary>
    [ObservableProperty]
    private string _rangoGrupos = string.Empty;

    /// <summary>Texto del contador "Condiciones en lista" de grupos (txtGruposLista).</summary>
    public string GruposEnListaTexto => RelacionesGrupos.Count.ToString();

    // ======================= Comandos: condiciones sencillas =======================

    [RelayCommand]
    private void AnadirCondicion()
    {
        // Construye los textos "Si" y "Entonces" a partir de los desplegables y el valor.
        // Validaciones y formato equivalentes a IfThenFrm.btnAdd_Click().
        // TODO(dominio): replicar exactamente el formato legacy ("Genérica: valor", "Dibujo i+j",
        //   prefijo "(NO) ", y la regla de que ambas condiciones deben ser diferentes).
        var si = ComponerTexto(CondicionEspecificaIf, CondicionGenericaIf, ValorIf, NegadoIf);
        var entonces = ComponerTexto(CondicionEspecificaThen, CondicionGenericaThen, ValorThen, NegadoThen);
        if (string.IsNullOrWhiteSpace(si) || string.IsNullOrWhiteSpace(entonces) || si == entonces)
        {
            return;
        }

        Relaciones.Add(new RelacionIfThenViewModel(si!, entonces!));
        OnPropertyChanged(nameof(CondicionesEnListaTexto));
        ResetEntradaCondicion();
    }

    [RelayCommand]
    private void BorrarCondicionesSeleccionadas()
    {
        // Equivale a IfThenFrm.btnBorrar_Click(): quita las filas marcadas.
        for (int i = Relaciones.Count - 1; i >= 0; i--)
        {
            if (Relaciones[i].Seleccionada)
            {
                Relaciones.RemoveAt(i);
            }
        }

        OnPropertyChanged(nameof(CondicionesEnListaTexto));
    }

    // ======================= Comandos: grupos =======================

    [RelayCommand]
    private void AnadirGrupo()
    {
        // Equivale a IfThenFrm.btnAddGrupo_Click(): los grupos deben ser diferentes.
        if (string.IsNullOrWhiteSpace(GrupoIf) || string.IsNullOrWhiteSpace(GrupoThen) || GrupoIf == GrupoThen)
        {
            return;
        }

        var si = (NegadoGrupoIf ? "(NO) " : string.Empty) + GrupoIf;
        var entonces = (NegadoGrupoThen ? "(NO) " : string.Empty) + GrupoThen;
        RelacionesGrupos.Add(new RelacionIfThenViewModel(si, entonces));
        OnPropertyChanged(nameof(GruposEnListaTexto));

        GrupoIf = null;
        GrupoThen = null;
        NegadoGrupoIf = false;
        NegadoGrupoThen = false;
    }

    [RelayCommand]
    private void BorrarGruposSeleccionados()
    {
        // Equivale a IfThenFrm.btnBorrarGrupo_Click().
        for (int i = RelacionesGrupos.Count - 1; i >= 0; i--)
        {
            if (RelacionesGrupos[i].Seleccionada)
            {
                RelacionesGrupos.RemoveAt(i);
            }
        }

        OnPropertyChanged(nameof(GruposEnListaTexto));
    }

    // ======================= Comandos: menú (MenuCondiciones legacy) =======================

    [RelayCommand]
    private void Aceptar()
    {
        // TODO(dominio): construir un ControladorIfThen con las relaciones y los rangos,
        //   validar (rango obligatorio si hay relaciones) y asignarlo a analizador.IfThen.
        //   Legacy IfThenFrm.menuCondiciones1_BOk() -> guardarCondicion().
    }

    [RelayCommand]
    private void Guardar()
    {
        // TODO(dominio): serializar el ControladorIfThen a archivo (.if/.xml).
        //   Legacy IfThenFrm.menuCondiciones1_BGuardar() -> ArchivoCondiciones.GuardaArchivo().
    }

    [RelayCommand]
    private void Abrir()
    {
        // TODO(dominio): leer un ControladorIfThen desde archivo y recargar las listas.
        //   Legacy IfThenFrm.menuCondiciones1_BAbrir() -> ArchivoCondiciones.LeeIfThen() + cargarDatos().
    }

    [RelayCommand]
    private void Copiar()
    {
        // TODO(dominio): guardar el filtro en un temporal (Temp/tmp.if) y habilitar Pegar.
        //   Legacy IfThenFrm.menuCondiciones1_BCopiar().
    }

    [RelayCommand]
    private void Pegar()
    {
        // TODO(dominio): cargar el filtro desde el temporal (Temp/tmp.if).
        //   Legacy IfThenFrm.menuCondiciones1_BPegar().
    }

    [RelayCommand]
    private void Borrar()
    {
        // Equivale a IfThenFrm.limpiar(): vacía ambas listas y los rangos.
        // TODO(dominio): pedir confirmación si el filtro no está vacío (ControladorIfThen.EsVacio).
        Relaciones.Clear();
        RelacionesGrupos.Clear();
        RangoCondiciones = string.Empty;
        RangoGrupos = string.Empty;
        ResetEntradaCondicion();
        OnPropertyChanged(nameof(CondicionesEnListaTexto));
        OnPropertyChanged(nameof(GruposEnListaTexto));
    }

    // ======================= Auxiliares =======================

    private void ResetEntradaCondicion()
    {
        // Equivale a IfThenFrm.resetConds().
        CondicionGenericaIf = null;
        CondicionEspecificaIf = null;
        CondicionesEspecificasIf = new List<string>();
        ValorIf = 0;
        NegadoIf = false;

        CondicionGenericaThen = null;
        CondicionEspecificaThen = null;
        CondicionesEspecificasThen = new List<string>();
        ValorThen = 0;
        NegadoThen = false;
    }

    private static string? ComponerTexto(string? especifica, string? generica, double valor, bool negado)
    {
        // Composición simplificada del texto de una condición. El formato definitivo
        // (incluido el caso "Dibujo i+j" sin valor) lo define el dominio legacy.
        var baseTexto = especifica ?? generica;
        if (string.IsNullOrWhiteSpace(baseTexto))
        {
            return null;
        }

        var texto = baseTexto.Contains("Dibujo")
            ? baseTexto
            : $"{baseTexto}: {valor:0}";
        return negado ? $"(NO) {texto}" : texto;
    }
}
