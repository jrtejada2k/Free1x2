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

    /// <summary>Acción para volver atrás (la cablea la página con Frame.GoBack()). CerrarVentana() legacy.</summary>
    public Action? Volver { get; set; }

    public SimetriasFrmViewModel()
    {
        // El form legacy crea 30 controles iniciales (LlenarControles(30)).
        for (int i = 1; i <= 30; i++)
        {
            Simetrias.Add(new FilaSimetria(i));
        }
    }

    /// <summary>
    /// Vuelca las simetrías del FiltroSimetrias del grupo en edición a las filas.
    /// Equivale a SimetriasFrm.MarcarValores() (Free1X2/UI/Filtros/SimetriasFrm.cs líneas 361-391).
    /// </summary>
    public void CargarDesdeGrupo()
    {
        var grupo = AppState.GrupoEnEdicion;
        if (grupo is null) return;

        var filtro = (FiltroSimetrias)grupo.GetFiltro(Filtro.Simetrias.ToString());
        if (!filtro.ContieneDatos) return;

        // Asegurar suficientes filas.
        while (Simetrias.Count < filtro.ArraySimetrias.Count)
        {
            Simetrias.Add(new FilaSimetria(Simetrias.Count + 1));
        }
        for (int i = 0; i < filtro.ArraySimetrias.Count; i++)
        {
            Simetrias[i].Partidos = filtro.ArraySimetrias[i].Partidos;
        }
        Aciertos = filtro.Aciertos;
    }

    // Valida una entrada de simetría (partidos 1..NumeroPartidos). SimetriasFrm.CompruebaEntradas, líneas 289-318.
    private static bool EntradaValida(string partidosSimetria)
    {
        if (string.IsNullOrWhiteSpace(partidosSimetria)) return false;
        try
        {
            bool esValida = false;
            foreach (string parte in partidosSimetria.Split(','))
            {
                foreach (string parte2 in parte.Split('-'))
                {
                    int a = Convert.ToInt32(parte2.Trim());
                    if (a <= 0 || a > VariablesGlobales.NumeroPartidos) return false;
                    esValida = true;
                }
            }
            return esValida;
        }
        catch
        {
            return false;
        }
    }

    // Construye la lista de Simetria (ObtenerSimetrias, líneas 253-277). Devuelve null si hay entrada inválida.
    private List<Simetria>? ObtenerSimetrias()
    {
        var lista = new List<Simetria>();
        foreach (var fila in Simetrias)
        {
            string txt = fila.Partidos;
            if (string.IsNullOrWhiteSpace(txt)) continue;
            if (!EntradaValida(txt)) return null;
            lista.Add(new Simetria(UtilidadesEntradasValores.ObtenerValoresSeparadosPorComas(txt)));
        }
        return lista;
    }

    // Parsea txtAciertos a List<int> (ObtenerAciertos, líneas 319-352). Devuelve null si el formato es inválido.
    private List<int>? ObtenerAciertos()
    {
        var lista = new List<int>();
        if (string.IsNullOrWhiteSpace(Aciertos)) return lista;
        try
        {
            foreach (string parte in Aciertos.Split(','))
            {
                string p = parte.Trim();
                if (p.LastIndexOf('-') == -1)
                {
                    int v = Convert.ToInt32(p);
                    if (v >= 0 && v <= 40) lista.Add(v);
                }
                else
                {
                    string[] intervalo = p.Split('-');
                    for (int j = Convert.ToInt32(intervalo[0]); j <= Convert.ToInt32(intervalo[1]); j++)
                    {
                        if (j >= 0 && j <= 40) lista.Add(j);
                    }
                }
            }
            return lista;
        }
        catch
        {
            return null;
        }
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
        // Equivale a SimetriasFrm.menuCondiciones1_BOk (Free1X2/UI/Filtros/SimetriasFrm.cs líneas 413-436).
        var grupo = AppState.GrupoEnEdicion;
        if (grupo is null) { Volver?.Invoke(); return; }

        var simetrias = ObtenerSimetrias();
        var aciertos = ObtenerAciertos();
        // Si hay una entrada inválida, el form legacy muestra un error y NO cierra.
        if (simetrias is null || aciertos is null) return;

        var filtro = (FiltroSimetrias)grupo.GetFiltro(Filtro.Simetrias.ToString());
        filtro.ArraySimetrias = simetrias;
        filtro.ArrayAciertos = aciertos;
        filtro.Aciertos = string.IsNullOrWhiteSpace(Aciertos) ? "0" : Aciertos;
        filtro.IsActive = filtro.ContieneDatos;

        grupo.ActivaFiltro(filtro);
        AppState.Instancia.NotificarCambio();
        Volver?.Invoke();
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
        // Equivale a menuCondiciones1_BBorrar: reinstanciar el filtro vacío en el grupo.
        var grupo = AppState.GrupoEnEdicion;
        grupo?.ActivaFiltro(new FiltroSimetrias());
    }

    [RelayCommand]
    private void Cancelar()
    {
        // Equivale a menuCondiciones1_BCancelar -> CerrarVentana() (sin aplicar cambios).
        Volver?.Invoke();
    }
}
