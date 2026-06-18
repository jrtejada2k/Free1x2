using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Free1X2.EntradaSalida;
using Free1X2.MotorCalculo;
using Free1X2.MotorCalculo.Estadisticas;
using Free1X2.Utils;
using Free1X2.WinUI.Services;
using Windows.Storage;
using Windows.Storage.Pickers;

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
/// Replica las entradas y la lógica del WinForms <c>GruposEquiposFrm</c>
/// (Free1X2/UI/Filtros/GruposEquiposFrm.cs), que define agrupaciones de equipos
/// para el filtro <c>FiltroGruposEquipos</c>:
///
/// - Pestaña "Grupos Equipos": una rejilla de 14 partidos donde se marcan los
///   equipos elegidos (casa/fuera) y se fijan Victorias / Empates / Derrotas /
///   Suma de Puntos esperados para ese grupo. Se navega entre varios grupos
///   (anterior / siguiente / eliminar).
/// - Pestaña "Relaciones": condiciones que relacionan grupos por su índice
///   ("Grupos Equipos") y suman Victorias / Empates / Derrotas / Puntos.
///   Se navega entre varias relaciones.
///
/// Cableado al motor real: el grupo a editar llega vía <c>AppState.GrupoEnEdicion</c>;
/// se trabaja sobre copias de <see cref="GrupoEquipos"/> / <see cref="RelacionGE1"/>
/// y al Aceptar se vuelcan al <see cref="FiltroGruposEquipos"/> del grupo, se activa la
/// condición y se notifica el cambio (equivale a <c>menuCondiciones1_BOk</c>).
/// La selección casa/fuera por partido se codifica como en <c>ObtenEquiposSeleccionados</c>
/// del WinForms: '0'=ninguno, '1'=casa, '2'=fuera, '3'=ambos (X).
/// </summary>
public partial class GruposEquiposFrmViewModel : ObservableObject
{
    public GruposEquiposFrmViewModel()
    {
        for (int i = 1; i <= NumeroPartidos; i++)
        {
            Partidos.Add(new PartidoEquipos
            {
                Numero = i,
                EquipoCasa = "Equipo casa " + i,
                EquipoFuera = "Equipo fuera " + i,
            });
        }

        CargarDesdeGrupo();
    }

    // VariablesGlobales.NumeroPartidos del proyecto legacy (quiniela de 14 + P15).
    private const int NumeroPartidos = 14;

    /// <summary>
    /// Los 14 partidos con sus dos casillas de selección.
    /// Equivale a las casillas creadas en <c>AñadirCasillas()</c>.
    /// </summary>
    public ObservableCollection<PartidoEquipos> Partidos { get; } = new();

    // ===== Estado del motor (legacy: grupo / filtroGE / arrayGE / arrayRelaciones1) =====

    // Grupo en edición entregado por la MainPage (legacy: parámetro Grupo grupo del ctor).
    private Grupo? _grupo;

    // Filtro de grupos de equipos del grupo en edición (legacy: filtroGE).
    private FiltroGruposEquipos? _filtroGE;

    // Copia de trabajo de los grupos de equipos (legacy: List<GrupoEquipos> arrayGE).
    private List<GrupoEquipos> _arrayGE = new();

    // Copia de trabajo de las relaciones (legacy: List<RelacionGE1> arrayRelaciones1).
    private List<RelacionGE1> _arrayRelaciones1 = new();

    // Índice 0-based del grupo en pantalla (legacy: int noGEPantalla).
    private int _noGEPantalla;

    // Índice 0-based de la relación en pantalla (legacy: int relGE1Pantalla).
    private int _relGE1Pantalla;

    /// <summary>Acción de cierre/volver (la cablea la página con Frame.GoBack()). Legacy: CerrarVentana().</summary>
    public Action? Volver { get; set; }

    /// <summary>Acción para navegar a otra página (la cablea la página con Frame.Navigate(tipo)).</summary>
    public Action<Type>? Navegar { get; set; }

    // Fichero temporal de copiar/pegar (legacy: Application.StartupPath + "/Temp/tmp.geq").
    private static string RutaTemporal =>
        Path.Combine(AppContext.BaseDirectory, "Temp", "tmp.geq");

    // Directorio de columnas ganadoras (legacy: Application.StartupPath + "/Ganadoras/").
    private static string DirectorioGanadoras =>
        Path.Combine(AppContext.BaseDirectory, "Ganadoras") + Path.DirectorySeparatorChar;

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

    // ================= Carga / guardado contra el motor =================

    /// <summary>
    /// Carga la copia de trabajo de grupos de equipos y relaciones del grupo en edición.
    /// Equivale a GruposEquiposFrm.InicializaDatos() + InicializaDatosRelacionesGE()
    /// (Free1X2/UI/Filtros/GruposEquiposFrm.cs líneas 152-158, 684-708).
    /// </summary>
    public void CargarDesdeGrupo()
    {
        _grupo = AppState.GrupoEnEdicion;

        if (_grupo is null)
        {
            _filtroGE = null;
            _arrayGE = new List<GrupoEquipos>();
            _arrayRelaciones1 = new List<RelacionGE1>();
            _noGEPantalla = 0;
            _relGE1Pantalla = 0;
            ActualizaDatosPantalla(0);
            ActualizaDatosPantRel1(0);
            LlenaEquipos();
            return;
        }

        // Legacy InicializaDatos: filtroGE = (FiltroGruposEquipos)grupo.GetFiltro("GruposEquipos").
        _filtroGE = (FiltroGruposEquipos)_grupo.GetFiltro(Filtro.GruposEquipos.ToString());
        _arrayGE = ObtenCopiaArrayGE(_filtroGE);
        _noGEPantalla = 0;
        ActualizaDatosPantalla(_noGEPantalla);

        // Legacy InicializaDatosRelacionesGE.
        InicializaDatosRelacionesGE();

        // Legacy LlenaEquipos (rellena los nombres de equipo desde el boleto).
        LlenaEquipos();
    }

    /// <summary>
    /// Réplica de GruposEquiposFrm.ObtenCopiaArrayGE (líneas 203-221): clona cada
    /// <see cref="GrupoEquipos"/> del filtro a una copia de trabajo editable.
    /// </summary>
    private static List<GrupoEquipos> ObtenCopiaArrayGE(FiltroGruposEquipos filtro)
    {
        var copia = new List<GrupoEquipos>();
        foreach (GrupoEquipos ge in filtro.GruposEquipos)
        {
            var geCopia = new GrupoEquipos
            {
                Pronosticos = ge.Pronosticos,
                SumaPuntos = ge.SumaPuntos,
                Victorias = ge.Victorias,
                Empates = ge.Empates,
                Derrotas = ge.Derrotas,
            };
            copia.Add(geCopia);
        }
        return copia;
    }

    /// <summary>
    /// Rellena los nombres de los equipos de cada partido.
    /// </summary>
    private void LlenaEquipos()
    {
        // TODO[dominio]: GruposEquiposFrm.LlenaEquipos + getEquipo (líneas 177-201) leían los
        //   nombres reales desde FormPadre.pronosticos.BuscarControl(n).EquipoCasa / EquipoFuera,
        //   es decir, del control de boleto de WinForms (Free1X2/UI/Controls/Pronosticos.cs +
        //   PartidoBoleto.cs). El motor (Free1X2.Domain: Grupo / GrupoPartidos / Analizador) NO
        //   almacena los nombres de equipo por partido — son un dato exclusivo de la capa de UI
        //   del boleto, aún no portada a WinUI. Mientras no exista un boleto WinUI que exponga
        //   los nombres por partido (p. ej. AppState.Instancia.Analizador.GruposPartidos[0] + un
        //   modelo de boleto con EquipoCasa/EquipoFuera), se mantienen los textos por defecto
        //   "Equipo casa N" / "Equipo fuera N". La selección casa/fuera (la parte funcional del
        //   filtro) sí está cableada al motor.
    }

    // ---- Pestaña "Grupos Equipos": pantalla <-> modelo ----

    /// <summary>
    /// Vuelca el grupo de equipos indicado a la pantalla (selección + objetivos).
    /// Equivale a GruposEquiposFrm.ActualizaDatosPantalla (líneas 223-241).
    /// </summary>
    private void ActualizaDatosPantalla(int noGE)
    {
        GrupoEquipos ge;
        if (_arrayGE.Count > 0 && noGE >= 0 && noGE < _arrayGE.Count)
        {
            ge = _arrayGE[noGE];
            GrupoActual = noGE + 1;
            TotalGrupos = _arrayGE.Count;
        }
        else
        {
            ge = new GrupoEquipos();
            GrupoActual = 1;
            TotalGrupos = 1;
        }

        PonerEquiposSeleccionados(ge.Pronosticos);
        SumaPuntos = ge.SumaPuntos;
        Victorias = ge.Victorias;
        Empates = ge.Empates;
        Derrotas = ge.Derrotas;

        OnPropertyChanged(nameof(PuedeRetrocederGrupo));
    }

    /// <summary>
    /// Marca las casillas casa/fuera según el pronóstico del grupo.
    /// Equivale a GruposEquiposFrm.PonerEquiposSeleccionados (líneas 542-595):
    /// '1'=casa, '2'=fuera, '3'=ambos (X).
    /// </summary>
    private void PonerEquiposSeleccionados(char[] c)
    {
        for (int i = 0; i < Partidos.Count; i++)
        {
            PartidoEquipos partido = Partidos[i];
            char signo = (c is not null && i < c.Length) ? c[i] : '\0';

            partido.CasaSeleccionado = signo == '1' || signo == '3';
            partido.FueraSeleccionado = signo == '2' || signo == '3';
        }
    }

    /// <summary>
    /// Construye el pronóstico (char[14]) a partir de la selección de pantalla.
    /// Equivale a GruposEquiposFrm.ObtenEquiposSeleccionados (líneas 597-619):
    /// código = 48 (+1 si casa, +2 si fuera) -> '0'/'1'/'2'/'3'.
    /// </summary>
    private char[] ObtenEquiposSeleccionados()
    {
        var c = new char[NumeroPartidos];
        for (int i = 0; i < NumeroPartidos; i++)
        {
            int n = 48;
            if (i < Partidos.Count)
            {
                if (Partidos[i].CasaSeleccionado) n += 1;
                if (Partidos[i].FueraSeleccionado) n += 2;
            }
            c[i] = Convert.ToChar(n);
        }
        return c;
    }

    /// <summary>
    /// True si hay alguna casilla casa/fuera marcada en pantalla.
    /// Equivale a GruposEquiposFrm.TieneGEDatos (líneas 621-635).
    /// </summary>
    private bool TieneGEDatos()
    {
        foreach (PartidoEquipos partido in Partidos)
        {
            if (partido.CasaSeleccionado || partido.FueraSeleccionado)
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Las entradas Victorias/Empates/Derrotas/SumaPuntos deben ser numéricas o vacías,
    /// pero no todas vacías. Equivale a GruposEquiposFrm.SonEntradasValidas (líneas 294-310).
    /// </summary>
    private bool SonEntradasValidas()
    {
        if (Victorias != "" || Empates != "" || Derrotas != "" || SumaPuntos != "")
        {
            if (UtilidadesEntradasValores.SonTodosNumeros(Victorias) &&
                UtilidadesEntradasValores.SonTodosNumeros(Empates) &&
                UtilidadesEntradasValores.SonTodosNumeros(Derrotas) &&
                UtilidadesEntradasValores.SonTodosNumeros(SumaPuntos))
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// True si el grupo tiene algún pronóstico activo.
    /// Equivale a GruposEquiposFrm.HayPronosticosActivos (líneas 327-339).
    /// </summary>
    private static bool HayPronosticosActivos(GrupoEquipos ge)
    {
        for (int i = 0; i < ge.Pronosticos.Length; i++)
        {
            if (ge.Pronosticos[i] != '\0' && ge.Pronosticos[i] != '0')
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Vuelca la pantalla al GrupoEquipos indicado.
    /// Equivale a GruposEquiposFrm.GuardaDatosGE (líneas 340-358). Sólo persiste los
    /// objetivos si las entradas son válidas; si no, deja un aviso en Estado en vez del
    /// MessageBox del WinForms (regla anti-bloqueo de WinUI).
    /// </summary>
    private void GuardaDatosGE(GrupoEquipos ge)
    {
        ge.Pronosticos = ObtenEquiposSeleccionados();
        ge.CalcularLongPronosticos();
        if (HayPronosticosActivos(ge))
        {
            if (SonEntradasValidas())
            {
                ge.Victorias = Victorias;
                ge.Empates = Empates;
                ge.Derrotas = Derrotas;
                ge.SumaPuntos = SumaPuntos;
            }
            else
            {
                Estado = "Hay errores en la entrada de datos del grupo";
            }
        }
    }

    /// <summary>
    /// Guarda el grupo en pantalla en la copia de trabajo (creándolo si hace falta).
    /// Equivale a GruposEquiposFrm.GuardarGEActual (líneas 276-293).
    /// </summary>
    private void GuardarGEActual()
    {
        GrupoEquipos ge;
        if (_noGEPantalla < _arrayGE.Count)
        {
            ge = _arrayGE[_noGEPantalla];
            GuardaDatosGE(ge);
        }
        else if (TieneGEDatos())
        {
            ge = new GrupoEquipos();
            _arrayGE.Add(ge);
            GuardaDatosGE(ge);
        }
    }

    /// <summary>
    /// Cambia el grupo seleccionado: guarda el actual, mueve el índice y crea el grupo si falta.
    /// Equivale a GruposEquiposFrm.CambiaGESelecionado (líneas 248-274).
    /// </summary>
    private void CambiaGESelecionado(int noGE)
    {
        GuardarGEActual();

        _noGEPantalla = noGE;

        if (_arrayGE.Count < noGE + 1)
        {
            _arrayGE.Add(new GrupoEquipos());
        }

        ActualizaDatosPantalla(noGE);
    }

    /// <summary>
    /// True si la última copia de trabajo no tiene ningún pronóstico (a borrar).
    /// Equivale a GruposEquiposFrm.NecesitaBorrarUltimoGE (líneas 637-654).
    /// </summary>
    private bool NecesitaBorrarUltimoGE()
    {
        GrupoEquipos ge = _arrayGE[_arrayGE.Count - 1];
        char[] c = ge.Pronosticos;
        for (int i = 0; i < c.Length; i++)
        {
            if (c[i] == '1' || c[i] == '2' || c[i] == '3')
            {
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// Vuelca la copia de trabajo al filtro y activa la condición.
    /// Equivale a GruposEquiposFrm.GuardarDatos (líneas 360-405).
    /// </summary>
    private void GuardarDatos()
    {
        if (_filtroGE is null) return;

        GuardarGEActual();

        if (_arrayGE.Count > 0)
        {
            // Borrar el último GE si quedó vacío.
            if (NecesitaBorrarUltimoGE())
            {
                _arrayGE.RemoveAt(_arrayGE.Count - 1);
            }
        }
        else
        {
            _filtroGE.IsActive = false;
        }

        if (!_filtroGE.ContieneDatos)
        {
            // Primera vez guardando datos: activar la condición si hay grupos.
            if (_arrayGE.Count > 0)
            {
                _filtroGE.ContieneDatos = true;
                _filtroGE.IsActive = true;
            }
            else
            {
                _filtroGE.IsActive = false;
            }
        }
        else
        {
            _filtroGE.ContieneDatos = true;
            _filtroGE.IsActive = true;
        }

        for (int i = 0; i < _arrayGE.Count; i++)
        {
            _arrayGE[i].CalcularLongPronosticos();
        }
        _filtroGE.GruposEquipos = _arrayGE;
    }

    // ---- Pestaña "Relaciones": pantalla <-> modelo ----

    /// <summary>
    /// Carga la copia de trabajo de relaciones del filtro y muestra la primera.
    /// Equivale a GruposEquiposFrm.InicializaDatosRelacionesGE (líneas 684-708).
    /// </summary>
    private void InicializaDatosRelacionesGE()
    {
        _arrayRelaciones1 = new List<RelacionGE1>();

        if (_filtroGE is not null)
        {
            List<RelacionGE1> relacionesGE = _filtroGE.RelacionesGE1.Relaciones;
            for (int i = 0; i < relacionesGE.Count; i++)
            {
                RelacionGE1 relGuardada = relacionesGE[i];
                var rel = new RelacionGE1
                {
                    GruposEquipos = relGuardada.GruposEquipos,
                    SumaVictorias = relGuardada.SumaVictorias,
                    SumaEmpates = relGuardada.SumaEmpates,
                    SumaDerrotas = relGuardada.SumaDerrotas,
                    SumaPuntos = relGuardada.SumaPuntos,
                };
                _arrayRelaciones1.Add(rel);
            }
        }

        _relGE1Pantalla = 0;
        ActualizaDatosPantRel1(_relGE1Pantalla);
    }

    /// <summary>
    /// Vuelca la relación indicada a la pantalla.
    /// Equivale a GruposEquiposFrm.ActualizaDatosPantRel1 (líneas 710-728).
    /// </summary>
    private void ActualizaDatosPantRel1(int relGE1)
    {
        RelacionGE1 rel;
        if (_arrayRelaciones1.Count > 0 && relGE1 >= 0 && relGE1 < _arrayRelaciones1.Count)
        {
            rel = _arrayRelaciones1[relGE1];
            RelacionActual = relGE1 + 1;
            TotalRelaciones = _arrayRelaciones1.Count;
        }
        else
        {
            rel = new RelacionGE1();
            RelacionActual = 1;
            TotalRelaciones = 1;
        }

        GruposEquiposRel = rel.GruposEquipos;
        SumaVictoriasRel = rel.SumaVictorias;
        SumaEmpatesRel = rel.SumaEmpates;
        SumaDerrotasRel = rel.SumaDerrotas;
        SumaPuntosRel = rel.SumaPuntos;

        OnPropertyChanged(nameof(PuedeRetrocederRelacion));
    }

    /// <summary>
    /// True si la pantalla de relación contiene datos válidos.
    /// Equivale a GruposEquiposFrm.TieneRelacion1Datos (líneas 730-747).
    /// </summary>
    private bool TieneRelacion1Datos()
    {
        if (GruposEquiposRel == "")
        {
            return false;
        }
        if (SumaVictoriasRel == "" && SumaEmpatesRel == "" &&
            SumaDerrotasRel == "" && SumaPuntosRel == "")
        {
            return false;
        }
        return true;
    }

    /// <summary>
    /// Las sumas de la relación deben ser numéricas o vacías, pero no todas vacías.
    /// Equivale a GruposEquiposFrm.SonEntradasValidasRelaciones (líneas 311-326).
    /// </summary>
    private bool SonEntradasValidasRelaciones()
    {
        if (SumaVictoriasRel != "" || SumaEmpatesRel != "" || SumaDerrotasRel != "" || SumaPuntosRel != "")
        {
            if (UtilidadesEntradasValores.SonTodosNumeros(SumaVictoriasRel) &&
                UtilidadesEntradasValores.SonTodosNumeros(SumaEmpatesRel) &&
                UtilidadesEntradasValores.SonTodosNumeros(SumaDerrotasRel) &&
                UtilidadesEntradasValores.SonTodosNumeros(SumaPuntosRel))
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Vuelca la pantalla a la relación indicada (si es válida).
    /// Equivale a GruposEquiposFrm.GuardaDatosRel1 (líneas 796-806).
    /// </summary>
    private void GuardaDatosRel1(RelacionGE1 rel)
    {
        if (TieneRelacion1Datos() && SonEntradasValidasRelaciones())
        {
            rel.GruposEquipos = GruposEquiposRel;
            rel.SumaVictorias = SumaVictoriasRel;
            rel.SumaEmpates = SumaEmpatesRel;
            rel.SumaDerrotas = SumaDerrotasRel;
            rel.SumaPuntos = SumaPuntosRel;
        }
    }

    /// <summary>
    /// Guarda la relación en pantalla en la copia de trabajo (creándola si hace falta).
    /// Equivale a GruposEquiposFrm.GuardarRelGE1Actual (líneas 777-794).
    /// </summary>
    private void GuardarRelGE1Actual()
    {
        RelacionGE1 rel;
        if (_relGE1Pantalla < _arrayRelaciones1.Count)
        {
            rel = _arrayRelaciones1[_relGE1Pantalla];
            GuardaDatosRel1(rel);
        }
        else if (TieneRelacion1Datos() && SonEntradasValidasRelaciones())
        {
            rel = new RelacionGE1();
            _arrayRelaciones1.Add(rel);
            GuardaDatosRel1(rel);
        }
    }

    /// <summary>
    /// Cambia la relación seleccionada: guarda la actual, mueve el índice y crea si falta.
    /// Equivale a GruposEquiposFrm.CambiaRelGE1Selecionado (líneas 749-775).
    /// </summary>
    private void CambiaRelGE1Selecionado(int relGE1)
    {
        GuardarRelGE1Actual();
        _relGE1Pantalla = relGE1;

        if (_arrayRelaciones1.Count < relGE1 + 1)
        {
            _arrayRelaciones1.Add(new RelacionGE1());
        }

        ActualizaDatosPantRel1(_relGE1Pantalla);
    }

    /// <summary>
    /// True si la última relación no contiene datos (a borrar).
    /// Equivale a GruposEquiposFrm.NecesitaBorrarUltimaRel1 (líneas 837-856).
    /// </summary>
    private bool NecesitaBorrarUltimaRel1()
    {
        RelacionGE1 rel = _arrayRelaciones1[_arrayRelaciones1.Count - 1];
        if (rel.GruposEquipos == "")
        {
            return true;
        }
        if (rel.SumaVictorias == "" && rel.SumaEmpates == "" &&
            rel.SumaDerrotas == "" && rel.SumaPuntos == "")
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// Vuelca la copia de trabajo de relaciones al filtro (conservando sólo las que tienen grupos).
    /// Equivale a GruposEquiposFrm.GuardarDatosRelacionesGE1 (líneas 808-835).
    /// </summary>
    private void GuardarDatosRelacionesGE1()
    {
        if (_filtroGE is null) return;

        GuardarRelGE1Actual();

        if (_arrayRelaciones1.Count > 0 && NecesitaBorrarUltimaRel1())
        {
            _arrayRelaciones1.RemoveAt(_arrayRelaciones1.Count - 1);
        }

        var relacionesGEFinal = new List<RelacionGE1>();
        for (int i = 0; i < _arrayRelaciones1.Count; i++)
        {
            RelacionGE1 rel = _arrayRelaciones1[i];
            if (rel.GruposEquipos != "")
            {
                relacionesGEFinal.Add(rel);
            }
        }

        _filtroGE.RelacionesGE1.Relaciones = relacionesGEFinal;
    }

    // ----- Navegación de grupos -----

    [RelayCommand]
    private void GrupoAnterior()
    {
        // Legacy GruposEquiposFrm.btnPrev_Click (líneas 1411-1414).
        if (_noGEPantalla > 0)
        {
            CambiaGESelecionado(_noGEPantalla - 1);
        }
    }

    [RelayCommand]
    private void GrupoSiguiente()
    {
        // Legacy GruposEquiposFrm.btnNext_Click (líneas 1416-1422): sólo avanza si hay datos.
        if (TieneGEDatos())
        {
            CambiaGESelecionado(_noGEPantalla + 1);
        }
    }

    [RelayCommand]
    private void EliminarGrupo()
    {
        // Legacy GruposEquiposFrm.btnEliminarGrupo_Click (líneas 1424-1451).
        if (_noGEPantalla == 0)
        {
            if (_arrayGE.Count > 0)
            {
                _arrayGE.RemoveAt(_noGEPantalla);
            }
        }
        else
        {
            _arrayGE.RemoveAt(_noGEPantalla);
            _noGEPantalla = _noGEPantalla - 1;
        }

        if (_arrayGE.Count == 0)
        {
            _arrayGE.Add(new GrupoEquipos());
        }

        ActualizaDatosPantalla(_noGEPantalla);
    }

    // ----- Navegación de relaciones -----

    [RelayCommand]
    private void RelacionAnterior()
    {
        // Legacy GruposEquiposFrm.btnPrevRel_Click (líneas 1453-1456).
        if (_relGE1Pantalla > 0)
        {
            CambiaRelGE1Selecionado(_relGE1Pantalla - 1);
        }
    }

    [RelayCommand]
    private void RelacionSiguiente()
    {
        // Legacy GruposEquiposFrm.btnNexRel_Click (líneas 1458-1464): sólo avanza si hay datos.
        if (TieneRelacion1Datos())
        {
            CambiaRelGE1Selecionado(_relGE1Pantalla + 1);
        }
    }

    [RelayCommand]
    private void EliminarRelacion()
    {
        // Legacy GruposEquiposFrm.btnEliminaGERel_Click (líneas 1466-1493).
        if (_relGE1Pantalla == 0)
        {
            if (_arrayRelaciones1.Count > 0)
            {
                _arrayRelaciones1.RemoveAt(_relGE1Pantalla);
            }
        }
        else
        {
            _arrayRelaciones1.RemoveAt(_relGE1Pantalla);
            _relGE1Pantalla = _relGE1Pantalla - 1;
        }

        if (_arrayRelaciones1.Count == 0)
        {
            _arrayRelaciones1.Add(new RelacionGE1());
        }

        ActualizaDatosPantRel1(_relGE1Pantalla);
    }

    // ----- Barra de comandos (menuCondiciones del WinForms) -----

    [RelayCommand]
    private void Aceptar()
    {
        // Legacy GruposEquiposFrm.menuCondiciones1_BOk (líneas 1507-1513):
        //   GuardarDatos() + GuardarDatosRelacionesGE1() + grupo.ActivaFiltro(filtroGE) + cerrar.
        if (_grupo is null || _filtroGE is null)
        {
            Volver?.Invoke();
            return;
        }

        GuardarDatos();
        GuardarDatosRelacionesGE1();

        // Legacy: FormPadre.analizador.GruposPartidos[GrupoPantalla].ActivaFiltro(filtroGE).
        // Aquí GrupoEnEdicion ES ese grupo del analizador compartido (AppState).
        _grupo.ActivaFiltro(_filtroGE);

        AppState.Instancia.NotificarCambio();
        Volver?.Invoke();
    }

    [RelayCommand]
    private void Borrar()
    {
        // Legacy GruposEquiposFrm.menuCondiciones1_BBorrar (líneas 1574-1587): tras confirmación
        //   reinicia filtroGE = new FiltroGruposEquipos() y recarga la pantalla. El MessageBox de
        //   confirmación del WinForms se omite en WinUI (la página puede añadir un ContentDialog).
        if (_grupo is null) return;

        GuardarDatos();

        _filtroGE = new FiltroGruposEquipos();
        _arrayGE = ObtenCopiaArrayGE(_filtroGE);
        _noGEPantalla = 0;
        _relGE1Pantalla = 0;
        ActualizaDatosPantalla(0);
        InicializaDatosRelacionesGE();
        ActualizaDatosPantRel1(0);
        Estado = "Datos del filtro borrados";
    }

    [RelayCommand]
    private void Cancelar()
    {
        // Legacy GruposEquiposFrm.menuCondiciones1_BCancelar (líneas 1515-1518): cerrar sin guardar.
        Volver?.Invoke();
    }

    // ----- Persistencia en disco (ArchivoCondiciones) -----

    /// <summary>
    /// Guarda en disco la condición de grupos de equipos del grupo en edición.
    /// Equivale a GruposEquiposFrm.guardar (Free1X2/UI/Filtros/GruposEquiposFrm.cs líneas 1562-1572):
    /// activa el filtro si tiene grupos y vuelca el FiltroGruposEquipos vía ArchivoCondiciones.GuardaArchivo.
    /// </summary>
    private void GuardarEn(string nombreArchivo)
    {
        if (_filtroGE is null) return;

        var archComb = new ArchivoCondiciones { NombreArchivo = nombreArchivo };
        if (_filtroGE.GruposEquipos.Count > 0)
        {
            _filtroGE.ContieneDatos = true;
            _filtroGE.IsActive = true;
        }
        archComb.GuardaArchivo(_filtroGE);
    }

    /// <summary>
    /// Abre la condición desde disco y recarga grupo / filtroGE / copias de trabajo.
    /// Equivale a GruposEquiposFrm.abrir (Free1X2/UI/Filtros/GruposEquiposFrm.cs líneas 1546-1560):
    /// ArchivoCondiciones.AbrirArchivoCombinacion + LeeCondicion + GetFiltro("GruposEquipos").
    /// </summary>
    private void AbrirDesde(string nombreArchivo)
    {
        var archComb = new ArchivoCondiciones();
        if (archComb.AbrirArchivoCombinacion(nombreArchivo))
        {
            _grupo = archComb.LeeCondicion();
            _filtroGE = (FiltroGruposEquipos)_grupo.GetFiltro(Filtro.GruposEquipos.ToString());
            _arrayGE = ObtenCopiaArrayGE(_filtroGE);
            _noGEPantalla = 0;
            _relGE1Pantalla = 0;
            ActualizaDatosPantalla(_noGEPantalla);
            InicializaDatosRelacionesGE();
        }
    }

    /// <summary>
    /// Construye un FiltroGruposEquipos temporal con el estado actual (grupos + relaciones) sin
    /// tocar el filtro real, para el cálculo de estadísticas.
    /// Equivale a GruposEquiposFrm.ObtenerFiltroTemporal (Free1X2/UI/Filtros/GruposEquiposFrm.cs líneas 407-496).
    /// </summary>
    private FiltroGruposEquipos ObtenerFiltroTemporal()
    {
        var filtroTemp = new FiltroGruposEquipos();
        var arrayGETemporal = new List<GrupoEquipos>();
        var arrayRelaciones1Temporal = new List<RelacionGE1>();
        arrayGETemporal.AddRange(_arrayGE);
        arrayRelaciones1Temporal.AddRange(_arrayRelaciones1);

        GrupoEquipos ge;
        if (_noGEPantalla < arrayGETemporal.Count)
        {
            ge = arrayGETemporal[_noGEPantalla];
            GuardaDatosGE(ge);
        }
        else if (TieneGEDatos())
        {
            ge = new GrupoEquipos();
            arrayGETemporal.Add(ge);
            GuardaDatosGE(ge);
        }

        if (arrayGETemporal.Count > 0)
        {
            // Borrar la última CP si no contiene datos (legacy: RemoveAt sobre _arrayGE, se conserva tal cual).
            if (NecesitaBorrarUltimoGETemporal(arrayGETemporal))
            {
                _arrayGE.RemoveAt(arrayGETemporal.Count - 1);
            }
        }

        if (filtroTemp.ContieneDatos == false && arrayGETemporal.Count > 0)
        {
            // Primera vez guardando datos: activar la condición.
            filtroTemp.ContieneDatos = true;
            filtroTemp.IsActive = true;
        }

        for (int i = 0; i < arrayGETemporal.Count; i++)
        {
            arrayGETemporal[i].CalcularLongPronosticos();
        }
        filtroTemp.GruposEquipos = arrayGETemporal;

        RelacionGE1 rel2;
        if (_relGE1Pantalla < arrayRelaciones1Temporal.Count)
        {
            rel2 = arrayRelaciones1Temporal[_relGE1Pantalla];
            GuardaDatosRel1(rel2);
        }
        else if (TieneRelacion1Datos())
        {
            rel2 = new RelacionGE1();
            arrayRelaciones1Temporal.Add(rel2);
            GuardaDatosRel1(rel2);
        }

        if (arrayRelaciones1Temporal.Count > 0)
        {
            // Borrar la última relación si no contiene datos.
            if (NecesitaBorrarUltimaRel1Temporal(arrayRelaciones1Temporal))
            {
                arrayRelaciones1Temporal.RemoveAt(arrayRelaciones1Temporal.Count - 1);
            }
        }

        var relacionesGEFinal = new List<RelacionGE1>();
        for (int i = 0; i < arrayRelaciones1Temporal.Count; i++)
        {
            RelacionGE1 rel = arrayRelaciones1Temporal[i];
            if (rel.GruposEquipos != "")
            {
                relacionesGEFinal.Add(rel);
            }
        }

        filtroTemp.RelacionesGE1.Relaciones = relacionesGEFinal;
        return filtroTemp;
    }

    /// <summary>
    /// True si la última copia temporal de grupos no contiene pronósticos (a borrar).
    /// Equivale a GruposEquiposFrm.NecesitaBorrarUltimoGETemporal (líneas 655-672).
    /// </summary>
    private static bool NecesitaBorrarUltimoGETemporal(List<GrupoEquipos> arrayGETemporal)
    {
        GrupoEquipos ge = arrayGETemporal[arrayGETemporal.Count - 1];
        char[] c = ge.Pronosticos;
        for (int i = 0; i < c.Length; i++)
        {
            if (c[i] == '1' || c[i] == '2' || c[i] == '3')
            {
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// True si la última relación temporal no contiene datos (a borrar).
    /// Equivale a GruposEquiposFrm.NecesitaBorrarUltimaRel1Temporal (líneas 857-876).
    /// </summary>
    private static bool NecesitaBorrarUltimaRel1Temporal(List<RelacionGE1> arrayRelaciones1Temporal)
    {
        RelacionGE1 rel = arrayRelaciones1Temporal[arrayRelaciones1Temporal.Count - 1];
        if (rel.GruposEquipos == "")
        {
            return true;
        }
        if (rel.SumaVictorias == "" && rel.SumaEmpates == "" &&
            rel.SumaDerrotas == "" && rel.SumaPuntos == "")
        {
            return true;
        }
        return false;
    }

    [RelayCommand]
    private async Task Guardar()
    {
        // Equivale a GruposEquiposFrm.menuCondiciones1_BGuardar + guardar
        //   (Free1X2/UI/Filtros/GruposEquiposFrm.cs líneas 1535-1544, 1562-1572):
        //   GuardarDatos() + SaveFileDialog (*.geq/*.xml) + ArchivoCondiciones.GuardaArchivo(filtroGE).
        GuardarDatos();

        var picker = new FileSavePicker
        {
            SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
            SuggestedFileName = "Grupos de equipos",
        };
        picker.FileTypeChoices.Add("Grupos de equipos", new List<string> { ".geq" });
        picker.FileTypeChoices.Add("Grupos de equipos (XML)", new List<string> { ".xml" });
        WinRT.Interop.InitializeWithWindow.Initialize(picker, AppServices.WindowHandle);

        StorageFile? file = await picker.PickSaveFileAsync();
        if (file == null) return;

        try
        {
            GuardarEn(file.Path);
            Estado = "Condición guardada en " + file.Name;
        }
        catch (Exception ex)
        {
            AppServices.MostrarError("No se pudo guardar: " + ex.Message);
        }
    }

    [RelayCommand]
    private async Task Abrir()
    {
        // Equivale a GruposEquiposFrm.menuCondiciones1_BAbrir + abrir
        //   (Free1X2/UI/Filtros/GruposEquiposFrm.cs líneas 1520-1533, 1546-1560):
        //   GuardarDatos() + OpenFileDialog (*.geq/*.xml) + ArchivoCondiciones.AbrirArchivoCombinacion/LeeCondicion.
        GuardarDatos();

        var picker = new FileOpenPicker { SuggestedStartLocation = PickerLocationId.DocumentsLibrary };
        picker.FileTypeFilter.Add(".geq");
        picker.FileTypeFilter.Add(".xml");
        WinRT.Interop.InitializeWithWindow.Initialize(picker, AppServices.WindowHandle);

        StorageFile? file = await picker.PickSingleFileAsync();
        if (file == null) return;

        try
        {
            AbrirDesde(file.Path);
            Estado = "Condición abierta de " + file.Name;
        }
        catch (Exception ex)
        {
            AppServices.MostrarError("No se pudo abrir: " + ex.Message);
        }
    }

    [RelayCommand]
    private void Copiar()
    {
        // Equivale a GruposEquiposFrm.menuCondiciones1_BCopiar (líneas 1589-1597):
        //   GuardarDatos() + guardar(StartupPath + "/Temp/tmp.geq") y habilitaba Pegar.
        GuardarDatos();
        try
        {
            string ruta = RutaTemporal;
            Directory.CreateDirectory(Path.GetDirectoryName(ruta)!);
            GuardarEn(ruta);
            Estado = "Condición copiada";
        }
        catch (Exception ex)
        {
            AppServices.MostrarError("No se pudo copiar: " + ex.Message);
        }
    }

    [RelayCommand]
    private void Pegar()
    {
        // Equivale a GruposEquiposFrm.menuCondiciones1_BPegar (líneas 1599-1609):
        //   GuardarDatos() + abrir(StartupPath + "/Temp/tmp.geq").
        GuardarDatos();
        if (File.Exists(RutaTemporal))
        {
            try
            {
                AbrirDesde(RutaTemporal);
                Estado = "Condición pegada";
            }
            catch (Exception ex)
            {
                AppServices.MostrarError("No se pudo pegar: " + ex.Message);
            }
        }
    }

    [RelayCommand]
    private void Estadisticas()
    {
        // Equivale a GruposEquiposFrm.menuCondiciones1_BEstadisticas (líneas 1624-1634):
        //   ObtenerFiltroTemporal() -> CalculadorEstadisticas.EstadisticasFiltro(..., "/Ganadoras/") -> VisorEstadisticas.
        FiltroGruposEquipos filtroTemp = ObtenerFiltroTemporal();

        var calc = new CalculadorEstadisticas();
        List<Estadistica> lista = calc.EstadisticasFiltro(filtroTemp, DirectorioGanadoras);

        VisorEstadisticasViewModel.UltimasEstadisticas = lista;
        Navegar?.Invoke(typeof(VisorEstadisticasPage));
    }
}
