// Free1X2 · WinUI 3 — WIN3
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Free1X2.MotorCalculo;
using Free1X2.WinUI.Services;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// ViewModel para la pantalla "Copiar Columnas".
/// Replica los campos de entrada del WinForms <c>CopiarCPFrm</c>:
/// un texto con los índices de columnas probables a copiar (txtCP) y una lista
/// multi-selección de grupos destino (cmbGrupo, un ListBox MultiExtended).
/// El usuario indica qué columnas (por índice, lista "1,3,5" o rango "1-5") quiere
/// copiar y a qué grupo(s) del boleto deben copiarse.
///
/// Cableado al motor real: los grupos destino salen de
/// <c>AppState.Instancia.Analizador.GruposPartidos</c> y el origen de las columnas es el
/// <see cref="FiltroColProbables"/> del grupo en edición (<c>AppState.GrupoEnEdicion</c>),
/// igual que el <c>grupoCP</c> que el WinForms recibía de ColProbablesFrm.
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

    /// <summary>Acción de cierre/volver (la cablea la página con Frame.GoBack()). Legacy: Close().</summary>
    public Action? Volver { get; set; }

    /// <summary>Acción para navegar a otra página (la cablea la página con Frame.Navigate(tipo)).</summary>
    public Action<Type>? Navegar { get; set; }

    // Índice del grupo en pantalla (legacy: noGrupoPantalla = mainFrm.NoGrupoPantalla).
    private int _grupoPantalla;

    // Lista de CPs de origen: las columnas del grupo en edición (legacy: grupoCP, recibido de ColProbablesFrm).
    private List<ColumnaProbable>? _grupoCP;

    public CopiarCPFrmViewModel()
    {
        CargarDesdeGrupo();
    }

    /// <summary>
    /// Puebla la lista de grupos destino y captura el origen de las columnas.
    /// Equivale a CopiarCPFrm.InicializarGruposDropDown() (Free1X2/UI/Filtros/CopiarCPFrm.cs líneas 67-89):
    ///   - el índice 0 se muestra como "Boleto Base"; el resto como su número de grupo (1, 2, ...);
    ///   - se preselecciona el grupo en pantalla (AppState.GrupoPantalla).
    /// </summary>
    public void CargarDesdeGrupo()
    {
        Grupos.Clear();

        GrupoPartidos grupos = AppState.Instancia.Analizador.GruposPartidos;
        int noGrupos = grupos.Count;
        for (int i = 0; i < noGrupos; i++)
        {
            Grupos.Add(i == 0 ? "Boleto Base" : i.ToString());
        }

        _grupoPantalla = AppState.Instancia.GrupoPantalla;

        // Origen de las columnas: el grupo en edición. Si no hay, el grupo en pantalla.
        Grupo? grupoOrigen = AppState.GrupoEnEdicion;
        if (grupoOrigen is null && _grupoPantalla >= 0 && _grupoPantalla < noGrupos)
        {
            grupoOrigen = grupos[_grupoPantalla];
        }

        if (grupoOrigen is not null)
        {
            var filtroOrigen = (FiltroColProbables)grupoOrigen.GetFiltro(Filtro.ColProbables.ToString());
            _grupoCP = filtroOrigen.ColProbables;
        }
    }

    /// <summary>
    /// Equivale a <c>btnCopiar_Click</c> -> <c>CopiarColumnas()</c> del WinForms:
    /// valida los índices y copia cada ColumnaProbable a los grupos seleccionados.
    /// (Free1X2/UI/Filtros/CopiarCPFrm.cs líneas 91-142.)
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

        if (_grupoCP is null)
        {
            Estado = "No hay columnas de origen (abra primero el editor de Columnas Probables).";
            return;
        }

        // Parseo de los índices con la lógica autocontenida del legacy (ObtenCP/EncuentraSeparador).
        int[] indexCP;
        try
        {
            indexCP = ObtenCP(Columnas);
        }
        catch
        {
            Estado = "Expresión de columnas no válida (usa \"5\", \"1,3,5\" o \"1-5\").";
            return;
        }

        if (!IndicesValidos(indexCP, _grupoCP.Count))
        {
            Estado = $"Algún índice supera el número de columnas disponibles ({_grupoCP.Count}).";
            return;
        }

        GrupoPartidos grupos = AppState.Instancia.Analizador.GruposPartidos;
        int copiadas = 0;

        // Recorre todos los grupos y copia a los que estén seleccionados (legacy: cmbGrupo.GetSelected).
        for (int numGrupo = 0; numGrupo < Grupos.Count; numGrupo++)
        {
            if (!GruposSeleccionados.Contains(Grupos[numGrupo]))
            {
                continue;
            }

            if (numGrupo < 0 || numGrupo >= grupos.Count)
            {
                continue;
            }

            Grupo grupoDestino = grupos[numGrupo];
            var filtroCP = (FiltroColProbables)grupoDestino.GetFiltro(Filtro.ColProbables.ToString());

            for (int i = 0; i < indexCP.Length; i++)
            {
                ColumnaProbable cp = _grupoCP[indexCP[i] - 1];

                // Clona la columna probable (legacy CopiarColumnas líneas 110-122).
                var cpCopia = new ColumnaProbable
                {
                    Pronosticos = cp.Pronosticos,
                };
                cpCopia.SetNoAciertos(cp.GetAciertos());
                cpCopia.SetNoAciertosSeguidos(cp.GetAciertosSeguidos());
                cpCopia.SetNoFallosSeguidos(cp.GetFallosSeguidos());

                if (cp.ToleranciaLocalActiva)
                {
                    cpCopia.SetACTol(cp.GetACTol());
                    cpCopia.SetACSTol(cp.GetACSTol());
                    cpCopia.SetFSTol(cp.GetFSTol());
                    cpCopia.SetTolerancias(cp.GetTolerancias());
                }

                if (_grupoPantalla == numGrupo)
                {
                    // Destino == grupo en pantalla: se añade a la lista de origen (legacy grupoCP.Add).
                    _grupoCP.Add(cpCopia);
                }
                else
                {
                    filtroCP.ColProbables.Add(cpCopia);
                    filtroCP.ContieneDatos = true;
                    filtroCP.IsActive = true;
                }
                copiadas++;
            }
        }

        AppState.Instancia.NotificarCambio();
        Estado = $"Copiadas {copiadas} columna(s) a {GruposSeleccionados.Count} grupo(s).";
        Volver?.Invoke();
    }

    // ===== Helpers de parseo/validación autocontenidos copiados de CopiarCPFrm =====

    // CopiarCPFrm.IndicesValidos: ningún índice supera el tamaño de la lista de origen.
    private static bool IndicesValidos(int[] indexCP, int indiceMaximo)
    {
        for (int i = 0; i < indexCP.Length; i++)
        {
            if (indexCP[i] > indiceMaximo || indexCP[i] < 1)
            {
                return false;
            }
        }
        return true;
    }

    // CopiarCPFrm.ObtenCP
    private static int[] ObtenCP(string valores)
    {
        string separador = EncuentraSeparador(valores);

        int[] indexCP;

        if (separador == ",")
        {
            string[] strIndexCP = valores.Split(',');
            indexCP = new int[strIndexCP.Length];
            for (int i = 0; i < strIndexCP.Length; i++)
            {
                indexCP[i] = Convert.ToInt32(strIndexCP[i]);
            }
        }
        else if (separador == "-")
        {
            string[] tempIndex = valores.Split('-');
            int indexMin = Convert.ToInt32(tempIndex[0]);
            int indexMax = Convert.ToInt32(tempIndex[1]);
            int noIndexes = (indexMax - indexMin) + 1;
            indexCP = new int[noIndexes];
            for (int i = 0; i < indexCP.Length; i++)
            {
                indexCP[i] = indexMin + i;
            }
        }
        else
        {
            indexCP = new int[1];
            indexCP[0] = Convert.ToInt32(valores);
        }

        return indexCP;
    }

    // CopiarCPFrm.EncuentraSeparador
    private static string EncuentraSeparador(string values)
    {
        string separador = "";
        foreach (char c in values)
        {
            switch (c)
            {
                case ',':
                    separador = ",";
                    break;
                case '-':
                    separador = "-";
                    break;
            }

            if (separador != "")
            {
                break;
            }
        }
        return separador;
    }

    /// <summary>
    /// Equivale a <c>btnCrearGrupos_Click</c> del WinForms: crea N grupos nuevos
    /// (con todos los partidos activos) y los añade a la lista de destinos.
    /// (Free1X2/UI/Filtros/CopiarCPFrm.cs líneas 386-403.)
    /// </summary>
    [RelayCommand]
    private void CrearGrupos()
    {
        // Equivale a CopiarCPFrm.btnCrearGrupos_Click (Free1X2/UI/Filtros/CopiarCPFrm.cs líneas
        //   386-403): el WinForms abría CrearGruposFrm (ShowDialog) para leer udNumGrupos y luego
        //   creaba ese número de grupos. CrearGruposFrmViewModel.Crear ya ejecuta ese mismo bucle
        //   (crea N grupos con los 14 partidos activos + NotificarCambio), así que aquí basta con
        //   navegar a la página. Al volver, OnNavigatedTo -> CargarDesdeGrupo refresca la lista de
        //   grupos destino con los recién creados.
        Navegar?.Invoke(typeof(CrearGruposFrmPage));
    }

    /// <summary>
    /// Equivale a <c>btnCancelar_Click</c> del WinForms (Close()).
    /// </summary>
    [RelayCommand]
    private void Cancelar()
    {
        Estado = "Cancelado.";
        Volver?.Invoke();
    }
}
