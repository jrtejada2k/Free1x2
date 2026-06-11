using System.Collections.ObjectModel;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// Elemento editable de una línea de "simetría" (un grupo de partidos).
/// En el form legacy es un control <c>CtrlSimetria</c> con un número y un TextBox
/// donde se escribe un grupo de partidos separados por "," o "-".
/// </summary>
public partial class SimetriaLinea : ObservableObject
{
    public SimetriaLinea(int numero)
    {
        _numero = numero;
    }

    // Número de orden de la línea (LblNum del CtrlSimetria legacy).
    [ObservableProperty]
    private int _numero;

    // string para evitar bindear int directo a TextBlock.Text (regla anti-crash 2).
    public string NumeroTexto => Numero.ToString();

    // Texto del grupo de partidos (TxtSimetria del CtrlSimetria legacy).
    [ObservableProperty]
    private string _valor = string.Empty;
}

/// <summary>
/// ViewModel del filtro "Diferencias" (port del WinForms <c>DiferenciasFrm</c>).
///
/// Propósito: el usuario define en cada línea un grupo de partidos (separados por ","
/// o por "-"). Sobre el conjunto de grupos se analiza, por concepto (Variantes, Equis,
/// Doses, Dibujos, Interrupciones, Formatos), la cantidad o el intervalo de valores
/// DISTINTOS entre los grupos especificados. El form permite guardar varios "conjuntos"
/// (cada uno es una <c>Diferencia</c> del <c>FiltroDiferencias</c>) y navegar entre ellos.
///
/// La lógica de dominio (FiltroDiferencias, Diferencia, ArchivoCondiciones,
/// CalculadorEstadisticas, Grupo, etc.) está marcada como TODO.
/// </summary>
public partial class DiferenciasFrmViewModel : ObservableObject
{
    private const int LineasIniciales = 20;

    public DiferenciasFrmViewModel()
    {
        Lineas = new ObservableCollection<SimetriaLinea>();
        for (int i = 1; i <= LineasIniciales; i++)
        {
            Lineas.Add(new SimetriaLinea(i));
        }
        AtajosDisponibles = new[]
        {
            "Dúos", "Tríos", "Cuartetos", "Quintetos",
            "Sextetos", "Septetos", "Octetos"
        };
        _atajoSeleccionado = AtajosDisponibles[0];
    }

    // Líneas de grupos de partidos (los CtrlSimetria del contenedor "cctrl" legacy).
    public ObservableCollection<SimetriaLinea> Lineas { get; }

    // ----- Conceptos analizados (los TextBox de la derecha del form legacy) -----
    // Cada uno admite una cantidad o intervalo de valores DISTINTOS entre grupos.

    // txtV — "Variantes". Equivale a Diferencia.AcV.
    [ObservableProperty]
    private string _variantes = string.Empty;

    // txtX — "Equis". Equivale a Diferencia.AcX.
    [ObservableProperty]
    private string _equis = string.Empty;

    // txtDoses — "Doses". Equivale a Diferencia.AcDoses.
    [ObservableProperty]
    private string _doses = string.Empty;

    // txtDib — "Dibujos". Equivale a Diferencia.AcDib.
    [ObservableProperty]
    private string _dibujos = string.Empty;

    // txtInt — "Interrupciones". Equivale a Diferencia.AcInt.
    [ObservableProperty]
    private string _interrupciones = string.Empty;

    // txtFormatos — "Formatos". Equivale a Diferencia.AcFormatos.
    [ObservableProperty]
    private string _formatos = string.Empty;

    // ----- Navegación de conjuntos (groupBox "Conjuntos - actual / guardados") -----

    // conjunto (1-based) e índice de la Diferencia actual.
    [ObservableProperty]
    private int _conjuntoActual = 1;

    // Total de conjuntos guardados en el filtro (filtro.Diferencias.Count).
    [ObservableProperty]
    private int _conjuntosGuardados;

    // lblNoSim — "actual / guardados". String para no bindear int a Text (regla 2).
    public string ContadorConjuntosTexto => $"{ConjuntoActual}/{ConjuntosGuardados}";

    // Habilita el botón "anterior" (btnPrevCP) sólo si hay conjunto previo.
    public bool PuedeIrAnterior => ConjuntoActual > 1;

    partial void OnConjuntoActualChanged(int value)
    {
        OnPropertyChanged(nameof(ContadorConjuntosTexto));
        OnPropertyChanged(nameof(PuedeIrAnterior));
    }

    partial void OnConjuntosGuardadosChanged(int value)
    {
        OnPropertyChanged(nameof(ContadorConjuntosTexto));
    }

    // ----- Atajos (cbbAtajos + chkPasoFijo + btnOK) -----

    // ItemsSource del ComboBox de atajos (regla anti-crash 3: lista desde el VM).
    public IReadOnlyList<string> AtajosDisponibles { get; }

    // cbbAtajos.Text — tipo de agrupación a generar.
    [ObservableProperty]
    private string _atajoSeleccionado;

    // chkPasoFijo — genera los grupos con paso fijo.
    [ObservableProperty]
    private bool _pasoFijo = true;

    // ----- Comandos -----

    [RelayCommand]
    private void GenerarAtajos()
    {
        // btnOK_Click del form legacy.
        // TODO: Dominio legacy — UtilidadesEntradasValores.ObtenerGruposDeValores(n, PasoFijo)
        //   según AtajoSeleccionado (Dúos=2 ... Octetos=8) y volcar las cadenas a las
        //   líneas vacías (AñadirFromList).
    }

    [RelayCommand]
    private void ConjuntoSiguiente()
    {
        // btnNextCP_Click del form legacy: guarda la Diferencia en pantalla y avanza,
        // creando una nueva si se llega al final.
        // TODO: Dominio legacy — CreaDiferencia() y filtro.Diferencias[indice]/.Add(...).
        ConjuntoActual++;
        if (ConjuntoActual > ConjuntosGuardados)
        {
            ConjuntosGuardados = ConjuntoActual;
        }
        LimpiarPantalla();
    }

    [RelayCommand]
    private void ConjuntoAnterior()
    {
        // btnPrevCP_Click del form legacy: guarda y retrocede al conjunto previo.
        // TODO: Dominio legacy — CreaDiferencia() + MarcarValores(filtro.Diferencias[indice]).
        if (ConjuntoActual > 1)
        {
            ConjuntoActual--;
        }
    }

    [RelayCommand]
    private void EliminarActual()
    {
        // btnEliminaActual_Click del form legacy: elimina la Diferencia actual.
        // TODO: Dominio legacy — filtro.Diferencias.RemoveAt(indice) + MarcarValores().
        if (ConjuntosGuardados > 0)
        {
            ConjuntosGuardados--;
        }
        if (ConjuntoActual > 1 && ConjuntoActual > ConjuntosGuardados)
        {
            ConjuntoActual--;
        }
        LimpiarPantalla();
    }

    [RelayCommand]
    private void Aceptar()
    {
        // menuCondiciones1_BOk del form legacy.
        // TODO: Dominio legacy — ActualizarDatos(); filtro.IsActive = filtro.ContieneDatos;
        //   FormPadre...ActivaFiltro(filtro); CerrarVentana().
    }

    [RelayCommand]
    private void Cancelar()
    {
        // menuCondiciones1_BCancelar del form legacy — cerrar sin aplicar.
        // TODO: Dominio legacy — CerrarVentana().
    }

    [RelayCommand]
    private void Estadisticas()
    {
        // menuCondiciones1_BEstadisticas del form legacy.
        // TODO: Dominio legacy — ActualizarDatos();
        //   CalculadorEstadisticas.EstadisticasFiltro(filtro, ".../Ganadoras/")
        //   y mostrar Estadisticas.VisorEstadisticas.
    }

    [RelayCommand]
    private void Guardar()
    {
        // menuCondiciones1_BGuardar del form legacy.
        // TODO: Dominio legacy — ActualizarDatos();
        //   ArchivoCondiciones.GuardaArchivo(filtro) (filtros *.dif / *.xml).
    }

    [RelayCommand]
    private void Abrir()
    {
        // menuCondiciones1_BAbrir del form legacy.
        // TODO: Dominio legacy — ArchivoCondiciones.AbrirArchivoCombinacion(...) +
        //   LeeCondicion() y volcar el FiltroDiferencias a la pantalla (MarcarValores).
    }

    [RelayCommand]
    private void Copiar()
    {
        // menuCondiciones1_BCopiar del form legacy.
        // TODO: Dominio legacy — ActualizarDatos(); guardar fichero temporal "Temp/tmp.rep".
    }

    [RelayCommand]
    private void Pegar()
    {
        // menuCondiciones1_BPegar del form legacy.
        // TODO: Dominio legacy — crear FiltroDiferencias nuevo, grupo.ActivaFiltro(filtro)
        //   y abrir el fichero temporal "Temp/tmp.rep".
    }

    [RelayCommand]
    private void Borrar()
    {
        // menuCondiciones1_BBorrar del form legacy: borra todos los datos del filtro.
        // TODO: Dominio legacy — filtro.Diferencias.Clear().
        LimpiarPantalla();
        ConjuntoActual = 1;
        ConjuntosGuardados = 0;
    }

    // LimpiarPantalla() del form legacy: vacía líneas y conceptos.
    private void LimpiarPantalla()
    {
        foreach (var linea in Lineas)
        {
            linea.Valor = string.Empty;
        }
        Variantes = string.Empty;
        Equis = string.Empty;
        Doses = string.Empty;
        Dibujos = string.Empty;
        Interrupciones = string.Empty;
        Formatos = string.Empty;
    }
}
