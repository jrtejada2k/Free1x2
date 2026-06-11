using System.Collections.ObjectModel;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// ViewModel del filtro "Simetrías".
/// Se da una simetría entre dos o más partidos cuando comparten el mismo signo.
/// Cada simetría es una lista de partidos separados por comas (a,b), guiones (a-b)
/// o una mezcla (a,b-c). El filtro mantiene además un campo "Aciertos" (intervalos
/// individuales o rangos: "1,3,5-7"). Equivale a los CtrlSimetria + txtAciertos del
/// WinForms SimetriasFrm. La lógica de dominio (FiltroSimetrias, Simetria,
/// ArchivoCondiciones, CalculadorEstadisticas) está marcada como TODO.
/// </summary>
public partial class SimetriasFrmViewModel : ObservableObject
{
    // La fila de simetría se define como clase pública de nivel superior en
    // FilaSimetria.cs (necesario para x:DataType="vm:FilaSimetria" en el DataTemplate).

    // Colección de filas de simetría. El form legacy arranca con 30 controles y va
    // añadiendo más conforme el usuario rellena el último (Añadir_Enter).
    public ObservableCollection<FilaSimetria> Simetrias { get; } = new();

    // Campo "Aciertos": individuales o rangos separados por comas (txtAciertos legacy).
    [ObservableProperty]
    private string _aciertos = string.Empty;

    public SimetriasFrmViewModel()
    {
        // El form legacy crea 30 controles iniciales (LlenarControles(30)).
        for (int i = 1; i <= 30; i++)
        {
            Simetrias.Add(new FilaSimetria(i));
        }
        // TODO: Dominio legacy — si filtro.ArraySimetrias tiene datos, volcar cada
        //   Simetria.Partidos en su fila y filtro.Aciertos en Aciertos (MarcarValores()).
    }

    // true si hay alguna simetría introducida (filtro.ContieneDatos legacy).
    public bool ContieneDatos
    {
        get
        {
            foreach (var fila in Simetrias)
            {
                if (!string.IsNullOrWhiteSpace(fila.Partidos))
                {
                    return true;
                }
            }
            return false;
        }
    }

    [RelayCommand]
    private void AnadirFila()
    {
        // Equivale a Añadir_Enter del form legacy: añade un nuevo CtrlSimetria al final.
        Simetrias.Add(new FilaSimetria(Simetrias.Count + 1));
    }

    [RelayCommand]
    private void Aceptar()
    {
        // TODO: Dominio legacy — validar entradas y volcar al FiltroSimetrias:
        //   ObtenerSimetrias(): por cada fila no vacía, CompruebaEntradas(texto) y
        //     Simetria(Utils.UtilidadesEntradasValores.ObtenerValoresSeparadosPorComas(texto)).
        //   ObtenerAciertos(): parsear "Aciertos" en arrayAciertos.
        //   filtro.ArraySimetrias = ...; filtro.ArrayAciertos = ...; filtro.Aciertos = Aciertos;
        //   filtro.IsActive = filtro.ContieneDatos;
        //   FormPadre.analizador.GruposPartidos[...].ActivaFiltro(filtro);
        //   (equivale a menuCondiciones1_BOk del SimetriasFrm legacy).
    }

    [RelayCommand]
    private void Estadisticas()
    {
        // TODO: Dominio legacy — construir FiltroSimetrias temporal (ObtenerFiltroTemporal)
        //   y llamar CalculadorEstadisticas.EstadisticasFiltro(filtroTemp, ".../Ganadoras/")
        //   mostrando el VisorEstadisticas (equivale a menuCondiciones2_BEstadisticas).
    }

    [RelayCommand]
    private void Guardar()
    {
        // TODO: Dominio legacy — ActualizarDatos() y ArchivoCondiciones.GuardaArchivo(filtro)
        //   a un .sim/.xml (equivale a menuCondiciones1_BGuardar -> guardar()).
    }

    [RelayCommand]
    private void Abrir()
    {
        // TODO: Dominio legacy — ArchivoCondiciones.AbrirArchivoCombinacion(...) + LeeCondicion()
        //   y volcar el FiltroSimetrias leído a estas filas (menuCondiciones1_BAbrir -> abrir()).
    }

    [RelayCommand]
    private void Copiar()
    {
        // TODO: Dominio legacy — ActualizarDatos() y guardar a Temp/tmp.sim; habilitar Pegar
        //   (equivale a menuCondiciones1_BCopiar).
    }

    [RelayCommand]
    private void Pegar()
    {
        // TODO: Dominio legacy — leer Temp/tmp.sim y volcar al filtro/pantalla
        //   (equivale a menuCondiciones1_BPegar).
    }

    [RelayCommand]
    private void Borrar()
    {
        // Equivale a menuCondiciones1_BBorrar + LimpiarPantalla() del form legacy.
        foreach (var fila in Simetrias)
        {
            fila.Partidos = string.Empty;
        }
        Aciertos = string.Empty;
        // TODO: Dominio legacy — filtro = new FiltroSimetrias(); grupo.ActivaFiltro(filtro).
    }

    [RelayCommand]
    private void Cancelar()
    {
        // TODO: Navegación — cerrar la página/ventana (equivale a CerrarVentana()).
    }
}
