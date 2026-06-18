// Free1X2 · WinUI 3 — WIN3
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// ViewModel para la pantalla "Filtrar por límites" (legacy: Free1X2.UI.DialogoFiltrarPorLimitesFrm).
///
/// El formulario legacy trabaja sobre una matriz <c>int[10,4]</c> llamada <c>extremos</c>,
/// con 10 filas (rangos de aciertos) y 4 columnas por fila:
///   [,0] posición inicial del rango de aciertos (lblextremoN)
///   [,1] posición final del rango de aciertos (lblextremoNd)
///   [,2] número mínimo de diferencias a eliminar (txdifN)
///   [,3] número máximo de diferencias a eliminar (txdifNd)
///
/// En el legacy las columnas [,0]/[,1] (rango de aciertos) se muestran como texto
/// (TextBox de solo lectura, calculadas por CoherenciarExtremos) y solo las columnas
/// [,2]/[,3] (diferencias) son realmente editables por el usuario.
///
/// Contrato con el llamador (legacy: el caller pasa int[,] pextremos al ctor y lee
/// la propiedad pública extremos + ValoresAceptados tras cerrar). Aquí lo replican
/// <see cref="Inicializar"/> (carga + CoherenciarExtremos) y <see cref="Extremos"/> /
/// <see cref="ValoresAceptados"/> (resultado tras "Aceptar").
/// </summary>
public partial class DialogoFiltrarPorLimitesFrmViewModel : ObservableObject
{
    // Filas mostradas y editadas (mapeo del int[10,4] 'extremos').
    public ObservableCollection<FilaLimite> Filas { get; } = new();

    // Matriz de resultado (legacy: campo público int[10,4] extremos).
    private int[,] _extremos = new int[10, 4];

    /// <summary>Matriz int[10,4] resultante (legacy: DialogoFiltrarPorLimitesFrm.extremos).</summary>
    public int[,] Extremos => _extremos;

    /// <summary>True si el usuario pulsó "Aceptar" (legacy: ValoresAceptados).</summary>
    public bool ValoresAceptados { get; private set; }

    /// <summary>Se dispara al aceptar/cancelar para que el host cierre el diálogo.</summary>
    public event EventHandler? CierreSolicitado;

    // Etiquetas de los 10 tramos de aciertos (legacy: label7/8/11/14/45/17/20/23/26 + fila 10).
    private static readonly string[] _etiquetas =
    {
        "< 10",
        "de 10 a < 11",
        "de 11 a < 12",
        "de 12 a < 13",
        "de 13 a < 14",
        "de < 13 a 12",
        "de < 12 a 11",
        "de < 11 a 10",
        "< 10",
        "fila 10",
    };

    public DialogoFiltrarPorLimitesFrmViewModel()
    {
        // Valores por defecto del Designer legacy (txdifN / txdifNd), por si la pantalla se
        // abre sin que el caller llame a Inicializar. Posiciones a 0 (se ajustan al inicializar).
        double[,] difsPorDefecto =
        {
            { 0, 4 }, { 1, 3 }, { 1, 2 }, { 1, 1 }, { 1, 0 },
            { 1, 0 }, { 1, 1 }, { 1, 2 }, { 0, 0 }, { 0, 0 },
        };
        for (int i = 0; i < _etiquetas.Length; i++)
        {
            Filas.Add(new FilaLimite(_etiquetas[i], 0, 0, difsPorDefecto[i, 0], difsPorDefecto[i, 1]));
            _extremos[i, 2] = (int)difsPorDefecto[i, 0];
            _extremos[i, 3] = (int)difsPorDefecto[i, 1];
        }
    }

    /// <summary>
    /// Carga la matriz 'extremos' que el caller pasaba por constructor (legacy:
    /// ctor DialogoFiltrarPorLimitesFrm(int[,] pextremos)). Aplica CoherenciarExtremos y
    /// construye las 10 filas con el mismo formato de texto que el form legacy:
    ///   fila 0 -> "extremos[0,0] a extremos[0,1]"
    ///   filas 1..9 -> "(extremos[i,0]+1) a extremos[i,1]"
    /// Las columnas [,2]/[,3] (diferencias) se cargan como editables.
    /// </summary>
    public void Inicializar(int[,] pextremos)
    {
        if (pextremos == null) return;
        _extremos = pextremos;
        CoherenciarExtremos();

        Filas.Clear();
        for (int i = 0; i < 10; i++)
        {
            // Posición inicial mostrada: fila 0 sin +1; resto con +1 (legacy ctor).
            int posIni = i == 0 ? _extremos[i, 0] : _extremos[i, 0] + 1;
            int posFin = _extremos[i, 1];
            Filas.Add(new FilaLimite(_etiquetas[i], posIni, posFin, _extremos[i, 2], _extremos[i, 3]));
        }
    }

    /// <summary>
    /// Equivalente a btAceptar_Click del legacy: vuelca los valores de UI a la matriz
    /// 'extremos' (PonerTextoEnVariables), re-coherencia (CoherenciarExtremos) y marca
    /// ValoresAceptados = true para el caller.
    /// </summary>
    [RelayCommand]
    private void Aceptar()
    {
        PonerTextoEnVariables();
        CoherenciarExtremos();
        ValoresAceptados = true;
        CierreSolicitado?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>Equivalente a button1_Click (Cancelar): cierra sin marcar ValoresAceptados.</summary>
    [RelayCommand]
    private void Cancelar()
    {
        ValoresAceptados = false;
        CierreSolicitado?.Invoke(this, EventArgs.Empty);
    }

    // Réplica de PonerTextoEnVariables(): vuelca posiciones y diferencias de cada fila a la
    // matriz. Las posiciones provienen de FilaLimite (solo lectura), las diferencias de la
    // edición del usuario. Nota legacy: la fila 10 (índice 9) reusaba los controles de la
    // fila 9 (lblextremo17/txdif17); aquí se respeta usando los valores de Filas[8].
    private void PonerTextoEnVariables()
    {
        for (int i = 0; i < Filas.Count && i < 10; i++)
        {
            var fila = Filas[i];
            _extremos[i, 0] = fila.PosInicial;
            _extremos[i, 1] = fila.PosFinal;
            _extremos[i, 2] = (int)fila.DifMin;
            _extremos[i, 3] = (int)fila.DifMax;
        }
        // Legacy: la fila 10 (índice 9) copiaba los valores de la fila 9 (índice 8).
        if (Filas.Count >= 9)
        {
            _extremos[9, 0] = Filas[8].PosInicial;
            _extremos[9, 1] = Filas[8].PosFinal;
            _extremos[9, 2] = (int)Filas[8].DifMin;
            _extremos[9, 3] = (int)Filas[8].DifMax;
        }
    }

    // Réplica de CoherenciarExtremos(): garantiza extremos[i+1,0] >= extremos[i,0]+1.
    private void CoherenciarExtremos()
    {
        for (int i = 0; i < 9; i++)
        {
            if (_extremos[i + 1, 0] < _extremos[i, 0] + 1)
            {
                _extremos[i + 1, 0] = _extremos[i, 0] + 1;
            }
        }
    }
}
