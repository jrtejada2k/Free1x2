// Free1X2 · WinUI 3 — WIN3
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Free1X2.Mascaras;
using Free1X2.Online;
using Free1X2.WinUI.Services;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>Una máscara guardada en la lista de la pantalla.</summary>
public sealed class FilaMascaraGuardada
{
    public FilaMascaraGuardada(Mascara m)
    {
        Mascara = m;
        Signos = string.Join(" ", m.Pronosticos.Select(p => p.Replace(",", "")));
        Resumen = "F " + m.Fijos + " · D " + m.Dobles + " · T " + m.Triples + " · " + m.Columnas + " col.";
    }

    public Mascara Mascara { get; }
    public string Nombre => Mascara.Nombre;

    /// <summary>Signos sin comas y separados por espacios («1X2 1X2 … 1»).</summary>
    public string Signos { get; }

    /// <summary>«F n · D n · T n · N col.»</summary>
    public string Resumen { get; }
}

/// <summary>Una liga del selector: «liga  ·  n jornadas».</summary>
public sealed record LigaHistorico(string Nombre, int Jornadas)
{
    public string Texto => Nombre + "  ·  " + Jornadas + " jornadas";
}

/// <summary>Una fila de la clasificación: columnas premiadas por categoría en todo el histórico.</summary>
public sealed class FilaResultadoMascara
{
    public FilaResultadoMascara(ResultadoMascara r)
    {
        Nombre = r.Nombre;
        Columnas = r.Columnas;
        P14 = r.Premio(14);
        P13 = r.Premio(13);
        P12 = r.Premio(12);
        P11 = r.Premio(11);
        P10 = r.Premio(10);
        JornadasConPremio = r.JornadasConPremio;
        Jornadas = r.Jornadas;
    }

    public string Nombre { get; }
    public long Columnas { get; }
    public long P14 { get; }
    public long P13 { get; }
    public long P12 { get; }
    public long P11 { get; }
    public long P10 { get; }
    public int JornadasConPremio { get; }
    public int Jornadas { get; }
    public string ConPremio => JornadasConPremio + " de " + Jornadas;
}

/// <summary>
/// «Máscaras» (W9 + W12, DT-84/DT-88; pantalla nueva, no existe en el WinForms): las máscaras
/// guardadas, aplicarlas al boleto y escrutarlas contra la base de ganadoras de una liga. Mismos
/// textos que la app Mac (<c>App/Pantallas/Combinacion/Mascaras.swift</c>).
/// </summary>
public partial class MascarasViewModel : ObservableObject
{
    private static string Datos => MascarasInicio.CarpetaDatos;

    public ObservableCollection<FilaMascaraGuardada> Mascaras { get; } = new();
    public ObservableCollection<LigaHistorico> Ligas { get; } = new();
    public ObservableCollection<FilaResultadoMascara> Resultados { get; } = new();

    // Columna y sentido de la ordenación de la tabla. Orden inicial: «10» descendente.
    private string _columnaOrden = "10";
    private bool _descendente = true;
    private List<FilaResultadoMascara> _filas = new();

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(SinMascaras))]
    [NotifyCanExecuteChangedFor(nameof(EscrutarCommand))]
    private int _numMascaras;

    public bool SinMascaras => NumMascaras == 0;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HayLigas), nameof(SinLigas))]
    private int _numLigas;

    public bool HayLigas => NumLigas > 0;
    public bool SinLigas => NumLigas == 0;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(EscrutarCommand), nameof(AnadirArchivoCommand), nameof(BorrarLigaCommand))]
    private LigaHistorico? _ligaSeleccionada;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HayEstado))]
    private string _estado = "";

    public bool HayEstado => Estado.Length > 0;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(EscrutarCommand), nameof(AnadirArchivoCommand), nameof(BorrarLigaCommand),
        nameof(DescargarCommand))]
    private bool _ocupado;

    [ObservableProperty]
    private string _escrutada = "";

    [ObservableProperty]
    private bool _hayResultados;

    public MascarasViewModel()
    {
        Recargar();
        string pais;
        try { pais = JornadaCache.PaisMasReciente() ?? "es"; }
        catch (Exception) { pais = "es"; }
        RecargarLigas(CasosHistorico.Liga(pais));
    }

    public void Recargar()
    {
        Mascaras.Clear();
        foreach (var m in MascarasInicio.Listar()) Mascaras.Add(new FilaMascaraGuardada(m));
        NumMascaras = Mascaras.Count;
    }

    private void RecargarLigas(string? seleccionar = null)
    {
        string? quiero = seleccionar ?? LigaSeleccionada?.Nombre;
        Ligas.Clear();
        foreach (var (nombre, jornadas) in CasosHistorico.Ligas(Datos)) Ligas.Add(new LigaHistorico(nombre, jornadas));
        NumLigas = Ligas.Count;
        LigaSeleccionada = Ligas.FirstOrDefault(l => l.Nombre == quiero) ?? Ligas.FirstOrDefault();
    }

    [RelayCommand]
    private async Task GuardarMascara()
    {
        if (await MascarasInicio.GuardarBoletoComoMascaraAsync()) Recargar();
    }

    public void Aplicar(FilaMascaraGuardada fila) => MascarasInicio.Aplicar(fila.Mascara);

    public async Task BorrarAsync(FilaMascaraGuardada fila)
    {
        if (!await AppServices.ConfirmarAsync("¿Borrar la máscara «" + fila.Nombre + "»?")) return;
        try
        {
            CasosMascaras.Borrar(fila.Nombre, Datos);
        }
        catch (Exception ex)
        {
            AppServices.MostrarError("No se pudo borrar la máscara: " + ex.Message);
        }
        _filas.RemoveAll(f => f.Nombre == fila.Nombre);
        Ordenar();
        Recargar();
    }

    private bool PuedeEscrutar() => !Ocupado && NumMascaras > 0 && (LigaSeleccionada?.Jornadas ?? 0) > 0;

    /// <summary>Escruta todas las máscaras contra la base de la liga elegida.</summary>
    [RelayCommand(CanExecute = nameof(PuedeEscrutar))]
    private void Escrutar()
    {
        string liga = LigaSeleccionada!.Nombre;
        var ganadoras = CasosHistorico.Leer(liga, Datos).Select(g => g.Ganadora).ToList();
        _filas = Mascaras.Select(f => new FilaResultadoMascara(CasosMascaras.Escrutar(f.Mascara, ganadoras))).ToList();
        Escrutada = ganadoras.Count + " jornadas escrutadas (liga «" + liga + "»).";
        Ordenar();
    }

    /// <summary>Pulsar una cabecera ordena por esa columna; otra vez, al revés.</summary>
    public void OrdenarPor(string columna)
    {
        if (_columnaOrden == columna) _descendente = !_descendente;
        else (_columnaOrden, _descendente) = (columna, columna != "Máscara");
        Ordenar();
    }

    private void Ordenar()
    {
        Func<FilaResultadoMascara, IComparable> clave = _columnaOrden switch
        {
            "Máscara" => f => f.Nombre,
            "Columnas" => f => f.Columnas,
            "14" => f => f.P14,
            "13" => f => f.P13,
            "12" => f => f.P12,
            "11" => f => f.P11,
            "Jornadas con premio" => f => f.JornadasConPremio,
            _ => f => f.P10,
        };
        var orden = _descendente ? _filas.OrderByDescending(clave) : _filas.OrderBy(clave);
        Resultados.Clear();
        foreach (var f in orden) Resultados.Add(f);
        HayResultados = Resultados.Count > 0;
    }

    /// <summary>Las ganadoras de clubprogol se suman a la liga «España» o «México» sin duplicar jornadas.</summary>
    [RelayCommand(CanExecute = nameof(NoOcupado))]
    private async Task Descargar(string pais)
    {
        Ocupado = true;
        Estado = "Descargando el histórico…";
        try
        {
            var h = await QuinielaOnlineService.Instancia.ObtenerHistoricoAsync(pais);
            var r = CasosHistorico.SumarDescarga(h, pais, Datos);
            RecargarLigas(CasosHistorico.Liga(pais));
            Estado = "clubprogol ▸ «" + LigaSeleccionada?.Nombre + "»: " + r.Mensaje;
        }
        catch (Exception ex)
        {
            Estado = "";
            Log.Error("MascarasViewModel.Descargar(" + pais + ")", ex);
            AppServices.MostrarError("No se pudo descargar el histórico: " + ex.Message);
        }
        finally
        {
            Ocupado = false;
        }
    }

    private bool NoOcupado() => !Ocupado;

    [RelayCommand]
    private async Task NuevaLiga()
    {
        string? nombre = await AppServices.PedirTextoAsync(
            "Nueva liga", "Después añade sus ganadoras desde un archivo.", "Nombre de la liga", "Crear");
        if (nombre is null) return;
        try
        {
            string n = CasosHistorico.Crear(nombre, Datos);
            RecargarLigas(n);
            Estado = "Liga «" + n + "» creada: añade sus ganadoras desde un archivo.";
        }
        catch (Exception ex)
        {
            AppServices.MostrarError("No se pudo crear la liga: " + ex.Message);
        }
    }

    private bool PuedeUsarLiga() => !Ocupado && LigaSeleccionada is not null;

    /// <summary>«Añadir ganadoras desde archivo…»: «jornada;columna» por línea; se suman a la liga elegida.</summary>
    [RelayCommand(CanExecute = nameof(PuedeUsarLiga))]
    private async Task AnadirArchivo()
    {
        var archivo = await PickerHelper.AbrirAsync(".txt", "*");
        if (archivo is null || LigaSeleccionada is null) return;
        string liga = LigaSeleccionada.Nombre;
        try
        {
            string texto = await Task.Run(() => File.ReadAllText(archivo.Path, Encoding.UTF8));
            var r = CasosHistorico.Importar(texto, liga, Datos);
            RecargarLigas();
            Estado = "«" + archivo.Name + "» ▸ «" + liga + "»: " + r.Mensaje;
        }
        catch (Exception ex)
        {
            AppServices.MostrarError("No se pudo añadir el archivo de ganadoras: " + ex.Message);
        }
    }

    [RelayCommand(CanExecute = nameof(PuedeUsarLiga))]
    private async Task BorrarLiga()
    {
        if (LigaSeleccionada is null) return;
        var (n, jornadas) = (LigaSeleccionada.Nombre, LigaSeleccionada.Jornadas);
        if (!await AppServices.ConfirmarAsync("¿Borrar la liga «" + n + "» y sus " + jornadas + " jornadas guardadas?"))
            return;
        try
        {
            CasosHistorico.Borrar(n, Datos);
            Estado = "Liga «" + n + "» borrada.";
        }
        catch (Exception ex)
        {
            AppServices.MostrarError("No se pudo borrar la liga: " + ex.Message);
        }
        RecargarLigas();
    }
}
