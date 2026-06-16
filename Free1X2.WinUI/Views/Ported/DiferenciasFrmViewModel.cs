using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Free1X2;
using Free1X2.MotorCalculo;
using Free1X2.Utils;
using Free1X2.WinUI.Services;

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

    // Índice 0-based de la Diferencia (conjunto) actualmente en pantalla (DiferenciasFrm.indice).
    private int _indice;

    /// <summary>Acción para volver atrás (la cablea la página con Frame.GoBack()). CerrarVentana() legacy.</summary>
    public Action? Volver { get; set; }

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

    private FiltroDiferencias? ObtenerFiltro()
    {
        var grupo = AppState.GrupoEnEdicion;
        if (grupo is null) return null;
        return (FiltroDiferencias)grupo.GetFiltro(Filtro.Diferencias.ToString());
    }

    /// <summary>
    /// Vuelca la primera Diferencia del FiltroDiferencias del grupo en edición a la pantalla.
    /// Equivale a DiferenciasFrm.MarcarValores() (Free1X2/UI/Filtros/DiferenciasFrm.cs líneas 112-151).
    /// </summary>
    public void CargarDesdeGrupo()
    {
        var filtro = ObtenerFiltro();
        if (filtro is null) return;

        _indice = 0;
        ConjuntoActual = 1;
        ConjuntosGuardados = filtro.Diferencias.Count;

        if (filtro.ContieneDatos && filtro.Diferencias.Count > 0)
        {
            MarcarValores(filtro.Diferencias[0]);
        }
        else
        {
            LimpiarPantalla();
        }
    }

    // Vuelca una Diferencia del dominio a la pantalla (MarcarValores(Diferencia), líneas 127-151).
    private void MarcarValores(Diferencia rep)
    {
        LimpiarPantalla();

        // Asegurar suficientes líneas en pantalla.
        while (Lineas.Count < rep.PartidosSimetricos.Count)
        {
            Lineas.Add(new SimetriaLinea(Lineas.Count + 1));
        }
        for (int i = 0; i < rep.PartidosSimetricos.Count; i++)
        {
            Lineas[i].Valor = UtilidadesEntradasValores.ObtenerTextoFromBool(rep.PartidosSimetricos[i]);
        }
        if (rep.AnalizaVX2Dib)
        {
            Variantes = UtilidadesEntradasValores.ObtenerTextoFromBool(rep.AcV);
            Equis = UtilidadesEntradasValores.ObtenerTextoFromBool(rep.AcX);
            Doses = UtilidadesEntradasValores.ObtenerTextoFromBool(rep.AcDoses);
            Dibujos = UtilidadesEntradasValores.ObtenerTextoFromBool(rep.AcDib);
        }
        if (rep.AnalizaInterrupciones)
        {
            Interrupciones = UtilidadesEntradasValores.ObtenerTextoFromBool(rep.AcInt);
        }
        if (rep.AnalizaFormatos)
        {
            Formatos = UtilidadesEntradasValores.ObtenerTextoFromBool(rep.AcFormatos);
        }
    }

    private static bool[] ActivarTodos(int longitud)
    {
        bool[] array = new bool[longitud];
        for (int i = 1; i < array.Length; i++) array[i] = true;
        return array;
    }

    // Construye una Diferencia a partir de la pantalla (DiferenciasFrm.CreaDiferencia, líneas 176-257).
    private Diferencia? CreaDiferencia()
    {
        try
        {
            var rep = new Diferencia();
            foreach (var linea in Lineas)
            {
                if (!string.IsNullOrWhiteSpace(linea.Valor))
                {
                    bool[] partidos = UtilidadesEntradasValores.ObtenerBoolArrayFromTxt(
                        linea.Valor, VariablesGlobales.NumeroPartidos + 1);
                    rep.AñadirPartidosSimetricos(partidos);
                }
            }
            int n = rep.PartidosSimetricos.Count + 1;
            rep.AcV = UtilidadesEntradasValores.ObtenerBoolArrayFromTxt(Variantes, n);
            rep.AcX = UtilidadesEntradasValores.ObtenerBoolArrayFromTxt(Equis, n);
            rep.AcDoses = UtilidadesEntradasValores.ObtenerBoolArrayFromTxt(Doses, n);
            rep.AcDib = UtilidadesEntradasValores.ObtenerBoolArrayFromTxt(Dibujos, n);
            rep.AcInt = UtilidadesEntradasValores.ObtenerBoolArrayFromTxt(Interrupciones, n);
            rep.AcFormatos = UtilidadesEntradasValores.ObtenerBoolArrayFromTxt(Formatos, n);

            if (rep.PartidosSimetricos.Count > 0)
            {
                rep.AnalizaVX2Dib = !(rep.AcV == null && rep.AcX == null && rep.AcDoses == null && rep.AcDib == null);
                if (rep.AcV == null) rep.AcV = ActivarTodos(n);
                if (rep.AcX == null) rep.AcX = ActivarTodos(n);
                if (rep.AcDoses == null) rep.AcDoses = ActivarTodos(n);
                if (rep.AcDib == null) rep.AcDib = ActivarTodos(n);
                if (rep.AcInt == null) { rep.AcInt = ActivarTodos(n); rep.AnalizaInterrupciones = false; }
                else rep.AnalizaInterrupciones = true;
                if (rep.AcFormatos == null) { rep.AcFormatos = ActivarTodos(n); rep.AnalizaFormatos = false; }
                else rep.AnalizaFormatos = true;
            }
            else
            {
                return null;
            }
            return rep;
        }
        catch
        {
            return null;
        }
    }

    // Guarda/actualiza la Diferencia de pantalla en el filtro (DiferenciasFrm.ActualizarDatos, líneas 258-279).
    private void ActualizarDatos(FiltroDiferencias filtro)
    {
        if (filtro.Diferencias.Count > _indice)
        {
            var s = CreaDiferencia();
            if (s != null) filtro.Diferencias[_indice] = s;
        }
        else
        {
            var s = CreaDiferencia();
            if (s != null) filtro.Diferencias.Add(s);
        }
    }

    private void RefrescarContador(FiltroDiferencias filtro)
    {
        ConjuntoActual = _indice + 1;
        ConjuntosGuardados = filtro.Diferencias.Count;
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
        // Equivale a DiferenciasFrm.btnOK_Click (líneas 526-556): genera grupos de valores
        //   según el atajo seleccionado y los vuelca a las líneas vacías (AñadirFromList).
        int cantidad = AtajoSeleccionado switch
        {
            "Dúos" => 2,
            "Tríos" => 3,
            "Cuartetos" => 4,
            "Quintetos" => 5,
            "Sextetos" => 6,
            "Septetos" => 7,
            "Octetos" => 8,
            _ => 0,
        };
        if (cantidad == 0) return;

        List<string> cadenas = UtilidadesEntradasValores.ObtenerGruposDeValores(cantidad, PasoFijo);
        // AñadirFromList: rellena las líneas vacías en orden (líneas 73-89).
        int index = 0;
        foreach (var linea in Lineas)
        {
            if (string.IsNullOrWhiteSpace(linea.Valor) && index < cadenas.Count)
            {
                linea.Valor = cadenas[index];
                index++;
            }
        }
    }

    [RelayCommand]
    private void ConjuntoSiguiente()
    {
        // Equivale a DiferenciasFrm.btnNextCP_Click (líneas 317-359): guarda el conjunto en
        //   pantalla y avanza, creando uno nuevo si se llega al final.
        var filtro = ObtenerFiltro();
        if (filtro is null) return;

        if (filtro.Diferencias.Count > _indice)
        {
            var s = CreaDiferencia();
            if (s != null) filtro.Diferencias[_indice] = s;
            _indice++;
            if (filtro.Diferencias.Count > _indice)
            {
                MarcarValores(filtro.Diferencias[_indice]);
            }
            else
            {
                LimpiarPantalla();
            }
        }
        else
        {
            var s = CreaDiferencia();
            if (s != null)
            {
                filtro.Diferencias.Add(s);
                _indice++;
                LimpiarPantalla();
            }
        }
        RefrescarContador(filtro);
    }

    [RelayCommand]
    private void ConjuntoAnterior()
    {
        // Equivale a DiferenciasFrm.btnPrevCP_Click (líneas 361-390): guarda y retrocede.
        var filtro = ObtenerFiltro();
        if (filtro is null) return;
        if (_indice <= 0) return;

        if (filtro.Diferencias.Count > _indice)
        {
            var s = CreaDiferencia();
            if (s != null)
            {
                filtro.Diferencias[_indice] = s;
                _indice--;
            }
            MarcarValores(filtro.Diferencias[_indice]);
        }
        else
        {
            _indice--;
            MarcarValores(filtro.Diferencias[_indice]);
        }
        RefrescarContador(filtro);
    }

    [RelayCommand]
    private void EliminarActual()
    {
        // Equivale a DiferenciasFrm.btnEliminaActual_Click (líneas 508-524).
        var filtro = ObtenerFiltro();
        if (filtro is null) return;

        if (filtro.Diferencias.Count > _indice)
        {
            filtro.Diferencias.RemoveAt(_indice);
            if (_indice > 0) _indice--;
        }
        else
        {
            LimpiarPantalla();
        }
        // MarcarValores() reposiciona en el conjunto 0.
        _indice = 0;
        if (filtro.ContieneDatos && filtro.Diferencias.Count > 0)
        {
            MarcarValores(filtro.Diferencias[0]);
        }
        else
        {
            LimpiarPantalla();
        }
        RefrescarContador(filtro);
    }

    [RelayCommand]
    private void Aceptar()
    {
        // Equivale a DiferenciasFrm.menuCondiciones1_BOk (líneas 280-292).
        var grupo = AppState.GrupoEnEdicion;
        if (grupo is null) { Volver?.Invoke(); return; }

        var filtro = (FiltroDiferencias)grupo.GetFiltro(Filtro.Diferencias.ToString());
        ActualizarDatos(filtro);
        filtro.IsActive = filtro.ContieneDatos;
        grupo.ActivaFiltro(filtro);
        AppState.Instancia.NotificarCambio();
        Volver?.Invoke();
    }

    [RelayCommand]
    private void Cancelar()
    {
        // Equivale a menuCondiciones1_BCancelar -> CerrarVentana() (sin aplicar cambios).
        Volver?.Invoke();
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
        // Equivale a DiferenciasFrm.menuCondiciones1_BBorrar (líneas 460-473).
        var filtro = ObtenerFiltro();
        filtro?.Diferencias.Clear();
        _indice = 0;
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
