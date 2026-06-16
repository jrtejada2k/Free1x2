using System;
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

        // Parseo de los índices con la lógica autocontenida del legacy (ObtenCP/EncuentraSeparador).
        int[] indices;
        try
        {
            indices = ObtenCP(Columnas);
        }
        catch
        {
            Estado = "Expresión de columnas no válida (usa \"5\", \"1,3,5\" o \"1-5\").";
            return;
        }

        // TODO(dominio): completar CopiarCPFrm.CopiarColumnas() — la copia real necesita el
        //   motor que no existe en esta capa aislada. Ver Free1X2/UI/Filtros/CopiarCPFrm.cs línea 91:
        //   - Validar con IndicesValidos contra el tamaño de grupoCP (la lista de CPs de origen).
        //   - Para cada grupo seleccionado (ObtenGrupo, MainForm.MotorCalculo.GruposPartidos),
        //     obtener su FiltroColProbables (Filtro.ColProbables) y, por cada índice, clonar la
        //     ColumnaProbable (Pronosticos, aciertos/seguidos/fallos y, si ToleranciaLocalActiva,
        //     las tolerancias) y añadirla:
        //       * si el grupo es el de pantalla -> grupoCP.Add(copia);
        //       * en otro caso -> filtroCP.ColProbables.Add(copia); ContieneDatos = true; IsActive = true.
        //   - Llamar a parentForm.CambiaCPSelecionado() y cerrar (Close()).

        Estado = $"Columnas válidas ({indices.Length}); copia a {GruposSeleccionados.Count} grupo(s) pendiente del motor (sin form padre).";
    }

    // ===== Helpers de parseo autocontenidos copiados de CopiarCPFrm =====

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
