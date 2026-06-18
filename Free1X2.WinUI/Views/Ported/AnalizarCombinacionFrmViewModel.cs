using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using Free1X2.MotorCalculo;
using Free1X2.Utils;
using Free1X2.WinUI.Controls;
using Free1X2.WinUI.Services;
using Microsoft.UI.Xaml;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// Estado de evaluación de una condición/nodo del árbol de análisis.
/// Réplica de los 3 iconos de la leyenda del WinForms "AnalizarCombinacionFrm":
/// ok.gif (acertada), exclamacion_blanco.bmp (aceptada por tolerancias) y cancelar.gif (fallada).
/// </summary>
public enum EstadoCondicion
{
    /// <summary>Condición acertada (icono ok.gif).</summary>
    Acertada,
    /// <summary>Condición aceptada por tolerancias (icono exclamacion_blanco.bmp).</summary>
    AceptadaPorTolerancias,
    /// <summary>Condición fallada (icono cancelar.gif).</summary>
    Fallada
}

/// <summary>
/// Nodo del árbol de análisis. Cada nodo representa una condición o filtro evaluado
/// contra la combinación, con su estado y posibles sub-condiciones (jerarquía del TreeView legacy).
/// </summary>
public partial class NodoAnalisisViewModel : ObservableObject
{
    [ObservableProperty]
    private string _titulo = string.Empty;

    [ObservableProperty]
    private EstadoCondicion _estado = EstadoCondicion.Acertada;

    /// <summary>Sub-nodos (hijos del nodo en el TreeView original).</summary>
    public ObservableCollection<NodoAnalisisViewModel> Hijos { get; } = new();

    /// <summary>Texto accesible del estado para AutomationProperties.</summary>
    public string EstadoTexto => Estado switch
    {
        EstadoCondicion.Acertada => "Condición acertada",
        EstadoCondicion.AceptadaPorTolerancias => "Condición aceptada por tolerancias",
        _ => "Condición fallada"
    };

    /// <summary>Estado del semáforo equivalente para el indicador visual del nodo.</summary>
    public EstadoSemaforo Semaforo => Estado switch
    {
        EstadoCondicion.Acertada => EstadoSemaforo.Verde,
        EstadoCondicion.AceptadaPorTolerancias => EstadoSemaforo.Neutro,
        _ => EstadoSemaforo.Rojo
    };

    partial void OnEstadoChanged(EstadoCondicion value)
    {
        OnPropertyChanged(nameof(EstadoTexto));
        OnPropertyChanged(nameof(Semaforo));
    }
}

/// <summary>
/// ViewModel de la página portada del WinForms "AnalizarCombinacionFrm".
/// Es un visor de resultados (sólo lectura): muestra el árbol de condiciones/filtros
/// evaluados contra una combinación y su estado (acertada / aceptada por tolerancias / fallada).
///
/// Cableado al motor real: porta 1:1 el algoritmo de Free1X2.Analisis.AnalisisCombinacion
/// (WinForms-only, Free1X2/Analisis/AnalisisCombinacion.cs), que recorre los grupos del
/// Analizador, evalúa filtros (IFiltro.AnalizarFallos), tolerancias, control de grupos/conjuntos
/// y el controlador If-Then, sustituyendo TreeNode/ImageIndex/Color por NodoAnalisisViewModel/
/// EstadoCondicion. Los datos llegan por el handoff estático (columna + Analizador + pronósticos).
/// </summary>
public partial class AnalizarCombinacionFrmViewModel : ObservableObject
{
    /// <summary>
    /// Handoff estático con los argumentos del análisis de fallos. Equivale a los parámetros del
    /// método legacy AnalisisCombinacion.AnalizarCombinacion(nombreCombinacion, columna, analizador,
    /// pronosticosBase). La Page lo lee al navegar (mismo patrón que EstucolFrmViewModel.UltimoInforme).
    /// Productor cableado: ColGanadoraFrmViewModel.Analizar fija este handoff y navega aquí.
    /// </summary>
    public static (string nombre, long columna, Analizador analizador, string[] pronosticos)? UltimoAnalisis { get; set; }

    /// <summary>
    /// Raíz del árbol de análisis (equivale a treeView1.Nodes del WinForms).
    /// </summary>
    public ObservableCollection<NodoAnalisisViewModel> Nodos { get; } = new();

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(MensajeVacioVisibility))]
    private bool _tieneResultados;

    /// <summary>Nombre de la combinación analizada (legacy: f2.Text = "Análisis de fallos: ...").</summary>
    [ObservableProperty]
    private string _tituloCombinacion = "Analizar combinación";

    /// <summary>
    /// Visibilidad del mensaje "sin resultados": visible mientras el árbol esté vacío.
    /// Se expone como <see cref="Visibility"/> para no bindear un bool directo a la UI.
    /// </summary>
    public Visibility MensajeVacioVisibility =>
        TieneResultados ? Visibility.Collapsed : Visibility.Visible;

    // Estado de fallos por grupo/conjunto (legacy: bool[] grupos, conjuntos, conjuntos2).
    private bool[] _grupos = Array.Empty<bool>();
    private bool[] _conjuntos = Array.Empty<bool>();
    private bool[] _conjuntos2 = Array.Empty<bool>();

    public AnalizarCombinacionFrmViewModel()
    {
        // El árbol se rellena a partir del análisis de la combinación que la pantalla anterior
        // deja en el handoff. La Page se reconstruye en cada navegación, así que basta leerlo aquí.
        var analisis = UltimoAnalisis;
        // Consumir y limpiar el handoff: si se vuelve a abrir la pantalla directamente desde el
        // menú (sin pasar por ColGanadora), no debe reutilizar un análisis antiguo, sino analizar
        // la combinación que el usuario tenga en pantalla en ese momento (modo independiente).
        UltimoAnalisis = null;
        if (analisis.HasValue)
        {
            AnalizarCombinacion(analisis.Value.nombre, analisis.Value.columna,
                analisis.Value.analizador, analisis.Value.pronosticos);
        }
        else
        {
            // Modo independiente: la pantalla se abre directamente desde el menú, sin que el flujo
            // de ColGanadora haya dejado un handoff. En vez de mostrarse vacía, se analiza la
            // combinación que el usuario tiene en pantalla (AppState.Analizador compartido), igual
            // que la rama "Analizar combinación en pantalla" del legacy ColGanadoraFrm. La columna a
            // analizar es la del propio boleto base, derivada de los pronósticos (equivale a
            // Analizador.InicializaPronosticoBase: pronosticoBase = ConvStrToLong(Pronosticos)).
            AnalizarCombinacionActual();
        }
    }

    /// <summary>
    /// Construye el árbol de fallos a partir de la combinación que el usuario tiene en pantalla
    /// (el <see cref="Analizador"/> compartido de <see cref="AppState"/>). Réplica de la rama
    /// "Analizar combinación en pantalla" de ColGanadoraFrm, usando como "columna ganadora" la
    /// propia columna base del boleto (derivada de los pronósticos), de modo que abrir la pantalla
    /// directamente ejecute un análisis real en lugar de quedarse vacía.
    /// </summary>
    private void AnalizarCombinacionActual()
    {
        var estado = AppState.Instancia;
        Analizador analizador = estado.Analizador;

        // Pronósticos de la combinación en pantalla. Si el Analizador aún no tiene pronósticos
        // (combinación nueva sin tocar), se usa la combinación abierta "1,X,2" para los partidos
        // configurados, igual que hace el motor al partir de un boleto en blanco.
        string[] pronosticos = analizador.Pronosticos;
        if (pronosticos == null || pronosticos.Length == 0)
        {
            int n = Free1X2.VariablesGlobales.NumeroPartidos;
            pronosticos = new string[n];
            for (int i = 0; i < n; i++)
            {
                pronosticos[i] = "1,X,2";
            }
        }

        // Columna a analizar = columna base del boleto (mismos bits que pronosticoBase del motor).
        long columna = UtilColumnas.ConvStrToLong(pronosticos);

        // Pronósticos base sin comas, en el formato que espera EvaluarPronosticos (igual que la
        // rama "Abrir combinación" de ColGanadoraFrm, que hace pronosticos[i].Replace(",", "")).
        string[] pronosticosBase = new string[pronosticos.Length];
        for (int i = 0; i < pronosticos.Length; i++)
        {
            pronosticosBase[i] = (pronosticos[i] ?? string.Empty).Replace(",", "");
        }

        string nombre = string.IsNullOrEmpty(estado.NombreArchivoComb)
            ? "Combinación en pantalla"
            : estado.NombreArchivoComb;

        AnalizarCombinacion(nombre, columna, analizador, pronosticosBase);
    }

    /// <summary>
    /// Porta 1:1 AnalisisCombinacion.AnalizarCombinacion (Free1X2/Analisis/AnalisisCombinacion.cs):
    /// construye el árbol de condiciones evaluadas contra la combinación. Sustituye TreeNode/Color/
    /// ImageIndex por NodoAnalisisViewModel/EstadoCondicion (rojo->Fallada, naranja->Aceptada, resto->Acertada).
    /// </summary>
    private void AnalizarCombinacion(string nombreCombinacion, long columna, Analizador analizador, string[] pronosticosBase)
    {
        Nodos.Clear();
        bool hayFallos = false;

        nombreCombinacion = System.IO.Path.GetFileName(nombreCombinacion ?? string.Empty);
        TituloCombinacion = "Análisis de fallos: " + nombreCombinacion;

        // Nodo raíz de combinación (legacy: nodoBase "Combinación").
        var nodoBase = new NodoAnalisisViewModel { Titulo = "Combinación", Estado = EstadoCondicion.Acertada };
        Nodos.Add(nodoBase);

        _grupos = new bool[analizador.GruposPartidos.Count];
        for (int i = 0; i < analizador.GruposPartidos.Count; i++)
        {
            Grupo g = analizador.GruposPartidos[i];
            string texto = "Grupo " + i;
            if (i == 0)
                texto = "Boleto base";
            else if (g.NombreGrupo.Length > 0)
                texto += " (" + g.NombreGrupo + ")";

            var nodoPrincipal = new NodoAnalisisViewModel { Titulo = texto, Estado = EstadoCondicion.Acertada };
            nodoBase.Hijos.Add(nodoPrincipal);

            // Si es el boleto base, comprueba el pronóstico (legacy: evaluarPronosticos).
            if (i == 0)
            {
                var nodoSecundario = new NodoAnalisisViewModel { Titulo = "Pronóstico de partidos" };
                string[]? evaluacionPronosticos = EvaluarPronosticos(columna, pronosticosBase);
                if (evaluacionPronosticos != null)
                {
                    nodoSecundario.Estado = EstadoCondicion.Fallada;
                    nodoBase.Estado = EstadoCondicion.Fallada;
                    nodoPrincipal.Estado = EstadoCondicion.Fallada;
                    nodoPrincipal.Hijos.Add(nodoSecundario);
                    for (int j = 0; j < evaluacionPronosticos.Length; j++)
                    {
                        int n = Convert.ToInt16(evaluacionPronosticos[j]);
                        nodoSecundario.Hijos.Add(new NodoAnalisisViewModel
                        {
                            Titulo = "Partido " + n + "  (" + pronosticosBase[n - 1] + ")",
                            Estado = EstadoCondicion.Fallada
                        });
                    }
                }
                else
                {
                    nodoSecundario.Estado = EstadoCondicion.Acertada;
                    nodoPrincipal.Hijos.Add(nodoSecundario);
                }
            }

            // Columna a analizar para este grupo (legacy: g.ColumnaGrupo(columna)).
            long columnaAAnalizar = g.ColumnaGrupo(columna);
            for (int f = 0; f < g.Filtros.Count; f++)
            {
                IFiltro filtro = g.Filtros[f];
                if (filtro.IsActive)
                {
                    var nodoSecundario = new NodoAnalisisViewModel
                    {
                        Titulo = BuscarFiltro(filtro.NombreFiltro),
                        Estado = EstadoCondicion.Acertada
                    };
                    string[] evaluacionFiltro = filtro.AnalizarFallos(columnaAAnalizar);
                    nodoPrincipal.Hijos.Add(nodoSecundario);
                    if (evaluacionFiltro != null)
                    {
                        nodoSecundario.Estado = EstadoCondicion.Acertada;
                        nodoPrincipal.Estado = EstadoCondicion.Acertada;
                        for (int j = 0; j < evaluacionFiltro.Length; j++)
                        {
                            string marca = evaluacionFiltro[j].Substring(0, 1);
                            if (marca == "*")
                            {
                                // No hay fallo.
                                nodoSecundario.Hijos.Add(new NodoAnalisisViewModel
                                {
                                    Titulo = evaluacionFiltro[j].Substring(1),
                                    Estado = EstadoCondicion.Acertada
                                });
                                // Si el filtro tiene tolerancias locales y estaba en naranja, vuelve a verde.
                                if ((filtro.NombreFiltro == Filtro.PesosNumericos || filtro.NombreFiltro == Filtro.ColProbables)
                                    && nodoSecundario.Estado == EstadoCondicion.AceptadaPorTolerancias)
                                {
                                    nodoSecundario.Estado = EstadoCondicion.Acertada;
                                }
                            }
                            else if (marca == "!")
                            {
                                // Fallo pendiente de tolerancias.
                                nodoSecundario.Hijos.Add(new NodoAnalisisViewModel
                                {
                                    Titulo = evaluacionFiltro[j].Substring(1),
                                    Estado = EstadoCondicion.AceptadaPorTolerancias
                                });
                                if (nodoSecundario.Estado != EstadoCondicion.Fallada)
                                {
                                    nodoSecundario.Estado = EstadoCondicion.AceptadaPorTolerancias;
                                }
                            }
                            else
                            {
                                // Fallo.
                                nodoSecundario.Hijos.Add(new NodoAnalisisViewModel
                                {
                                    Titulo = evaluacionFiltro[j],
                                    Estado = EstadoCondicion.Fallada
                                });
                                nodoSecundario.Estado = EstadoCondicion.Fallada;
                                nodoPrincipal.Estado = EstadoCondicion.Fallada;
                                nodoBase.Estado = EstadoCondicion.Fallada;
                            }
                        }
                    }
                }
            }

            // Tolerancias globales del grupo (legacy: g.AnalizaToleranciasGrupo).
            if (g.ControladorTolerancias.Tolerancias.Count > 0 && !g.EsGrupoBase)
            {
                var nodoTolerancias = new NodoAnalisisViewModel { Titulo = "Tolerancias Globales" };
                if (g.AnalizaToleranciasGrupo(columnaAAnalizar))
                {
                    nodoPrincipal.Estado = EstadoCondicion.Acertada;
                    nodoTolerancias.Estado = EstadoCondicion.Acertada;
                }
                else
                {
                    nodoPrincipal.Estado = EstadoCondicion.Fallada;
                    nodoTolerancias.Estado = EstadoCondicion.Fallada;
                    nodoTolerancias.Titulo += " - Fallo en tolerancias Globales";
                    nodoPrincipal.Hijos.Add(nodoTolerancias);
                }
            }

            if (nodoPrincipal.Estado == EstadoCondicion.Fallada)
            {
                _grupos[i] = false;
                hayFallos = true;
            }
            else
            {
                _grupos[i] = true;
            }
        }

        if (hayFallos)
        {
            nodoBase.Estado = EstadoCondicion.Fallada;
        }

        // Control de grupos (legacy: nodoBase "Control de grupos").
        _conjuntos = new bool[analizador.CtrlGrupos.ControlesGrupos.Count];
        if (_conjuntos.Length > 1)
        {
            var nodoBaseGrupos = new NodoAnalisisViewModel { Titulo = "Control de grupos", Estado = EstadoCondicion.Acertada };
            Nodos.Add(nodoBaseGrupos);
            hayFallos = false;
            for (int i = 1; i < _conjuntos.Length; i++)
            {
                ControlGrupos cg = analizador.CtrlGrupos.ControlesGrupos[i];
                string texto = "Control de Grupos " + i;
                EstadoCondicion estado;
                if (!AnalizaFallosGrupos(cg))
                {
                    _conjuntos[i] = false;
                    hayFallos = true;
                    estado = EstadoCondicion.Fallada;
                }
                else
                {
                    _conjuntos[i] = true;
                    estado = EstadoCondicion.Acertada;
                    // Si el control permite los fallos producidos, los grupos afectados que estaban
                    // en rojo pasan a ámbar.
                    for (int j = 0; j < cg.GruposControlados.Length; j++)
                    {
                        NodoAnalisisViewModel nodeTemp = Nodos[0].Hijos[cg.GruposControlados[j]];
                        if (nodeTemp.Estado == EstadoCondicion.Fallada)
                        {
                            nodeTemp.Estado = EstadoCondicion.AceptadaPorTolerancias;
                        }
                    }
                }
                nodoBaseGrupos.Hijos.Add(new NodoAnalisisViewModel { Titulo = texto, Estado = estado });
            }
            if (hayFallos)
            {
                nodoBaseGrupos.Estado = EstadoCondicion.Fallada;
            }
            else if (_grupos.Length > 0 && _grupos[0])
            {
                Nodos[0].Estado = EstadoCondicion.AceptadaPorTolerancias;
            }

            // Control de conjuntos (legacy: nodoBase "Control de conjuntos").
            _conjuntos2 = new bool[analizador.CtrlGrupos.ControlesConjuntos.Count];
            if (_conjuntos2.Length > 1)
            {
                var nodoBaseConj = new NodoAnalisisViewModel { Titulo = "Control de conjuntos", Estado = EstadoCondicion.Acertada };
                Nodos.Add(nodoBaseConj);
                hayFallos = false;
                for (int i = 1; i < analizador.CtrlGrupos.ControlesConjuntos.Count; i++)
                {
                    ControlConjuntos cc = analizador.CtrlGrupos.ControlesConjuntos[i];
                    string texto = "Control de Conjuntos " + i;
                    EstadoCondicion estado;
                    if (!AnalizaFallosConjuntos(cc))
                    {
                        _conjuntos2[i] = false;
                        hayFallos = true;
                        estado = EstadoCondicion.Fallada;
                    }
                    else
                    {
                        _conjuntos2[i] = true;
                        estado = EstadoCondicion.Acertada;
                        int[] conj = cc.ObtenCtrolGruposConjunto();
                        for (int j = 0; j < conj.Length; j++)
                        {
                            NodoAnalisisViewModel nodeTemp = Nodos[1].Hijos[conj[j] - 1];
                            if (nodeTemp.Estado == EstadoCondicion.Fallada)
                            {
                                nodeTemp.Estado = EstadoCondicion.AceptadaPorTolerancias;
                            }
                        }
                    }
                    nodoBaseConj.Hijos.Add(new NodoAnalisisViewModel { Titulo = texto, Estado = estado });
                }
                if (hayFallos)
                {
                    nodoBaseConj.Estado = EstadoCondicion.Fallada;
                }
                else
                {
                    if (_grupos.Length > 0 && _grupos[0])
                    {
                        Nodos[0].Estado = EstadoCondicion.AceptadaPorTolerancias;
                    }
                    Nodos[1].Estado = EstadoCondicion.AceptadaPorTolerancias;
                }
            }
        }

        // Controlador If-Then (legacy: nodoBase "Condiciones relacionadas").
        if (analizador.IfThen != null && !analizador.IfThen.EsVacio && analizador.IfThen.EsActivo)
        {
            var nodoBaseIf = new NodoAnalisisViewModel { Titulo = "Condiciones relacionadas", Estado = EstadoCondicion.Acertada };
            Nodos.Add(nodoBaseIf);

            if (analizador.IfThen.ControlesCondiciones.Count > 0)
            {
                var nodoPrincipal = new NodoAnalisisViewModel { Titulo = "Condiciones sencillas", Estado = EstadoCondicion.Acertada };
                nodoBaseIf.Hijos.Add(nodoPrincipal);
                string[] evaluacionFiltro = analizador.IfThen.CompruebaErrores(columna);
                hayFallos = AgregarNodosIfThen(nodoPrincipal, evaluacionFiltro);
                if (hayFallos)
                {
                    nodoPrincipal.Estado = EstadoCondicion.Fallada;
                    nodoBaseIf.Estado = EstadoCondicion.Fallada;
                }
            }
            if (analizador.IfThen.ControlesGrupos.Count > 0)
            {
                var nodoPrincipal = new NodoAnalisisViewModel { Titulo = "Grupos relacionados", Estado = EstadoCondicion.Acertada };
                nodoBaseIf.Hijos.Add(nodoPrincipal);
                string[] evaluacionFiltro = analizador.IfThen.CompruebaErrores(columna, analizador.GruposPartidos);
                hayFallos = AgregarNodosIfThen(nodoPrincipal, evaluacionFiltro);
                if (hayFallos)
                {
                    nodoPrincipal.Estado = EstadoCondicion.Fallada;
                    nodoBaseIf.Estado = EstadoCondicion.Fallada;
                }
            }
        }

        TieneResultados = Nodos.Count > 0;
    }

    // Añade los nodos hijos de un control If-Then y devuelve si hubo algún fallo (legacy: switch sobre el prefijo).
    private static bool AgregarNodosIfThen(NodoAnalisisViewModel padre, string[] evaluacionFiltro)
    {
        bool hayFallos = false;
        for (int n = 0; n < evaluacionFiltro.Length; n++)
        {
            string car = evaluacionFiltro[n].Substring(0, 1);
            EstadoCondicion estado;
            string texto;
            switch (car)
            {
                case "*":
                    estado = EstadoCondicion.Acertada;
                    texto = evaluacionFiltro[n].Substring(1);
                    break;
                case "!":
                    estado = EstadoCondicion.AceptadaPorTolerancias;
                    texto = evaluacionFiltro[n].Substring(1);
                    break;
                default:
                    hayFallos = true;
                    estado = EstadoCondicion.Fallada;
                    texto = evaluacionFiltro[n];
                    break;
            }
            padre.Hijos.Add(new NodoAnalisisViewModel { Titulo = texto, Estado = estado });
        }
        return hayFallos;
    }

    // ===== Helpers portados 1:1 de AnalisisCombinacion =====

    private static string[]? EvaluarPronosticos(long cg, string[] pronosticos)
    {
        string[]? arrayFallos = null;
        int partido = 0;
        string fallos = "";
        while (cg != 0)
        {
            int signo = (int)cg & 7;
            int pronostico = UtilColumnas.ConvPartidoStrToByte(pronosticos[partido]);
            if ((signo & pronostico) != signo)
            {
                fallos = "#" + Convert.ToString(partido + 1) + fallos;
            }
            cg >>= 3;
            partido++;
        }
        if (fallos.Length >= 2)
        {
            string fallosDef = fallos.Substring(1, fallos.Length - 1);
            arrayFallos = fallosDef.Split('#');
        }
        return arrayFallos;
    }

    private static string BuscarFiltro(Filtro nombreFiltro) => nombreFiltro switch
    {
        Filtro.ColProbables => "Columnas probables",
        Filtro.FormatosSignos => "Formatos de Signos",
        Filtro.GruposEquipos => "Grupos de Equipos",
        Filtro.NoInterrupciones => "Interrupciones",
        Filtro.NoVariantes => "Variantes, X y 2",
        Filtro.PesosNumericos => "Pesos Numericos",
        Filtro.SignosSeguidos => "Signos Seguidos",
        Filtro.ValoracionSignos => "Valoracion de Signos",
        _ => nombreFiltro.ToString()
    };

    private bool AnalizaFallosGrupos(ControlGrupos c)
    {
        int numFallos = 0;
        for (int i = 0; i < c.GruposControlados.Length; i++)
        {
            int numGrupo = c.GruposControlados[i];
            if (!_grupos[numGrupo]) numFallos++;
        }
        return c.FallosPermitidos[numFallos];
    }

    private bool AnalizaFallosConjuntos(ControlConjuntos c)
    {
        int numFallos = 0;
        bool[] permitidos = c.ObtenFallosPermitidos();
        int[] conj = c.ObtenCtrolGruposConjunto();
        for (int i = 0; i < conj.Length; i++)
        {
            int numConjunto = conj[i];
            if (!_conjuntos[numConjunto]) numFallos++;
        }
        return permitidos[numFallos];
    }
}
