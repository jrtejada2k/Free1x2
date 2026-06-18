// Free1X2 · WinUI 3 — WIN3
using System;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Free1X2.MotorCalculo;
using Free1X2.WinUI.Services;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// ViewModel para la pantalla "Control de Grupos".
/// Replica el WinForms <c>ControlGruposFrm</c> (Free1X2/UI/Filtros/ControlGruposFrm.cs),
/// que opera sobre el <see cref="ControladorGrupos"/> de la combinación actual
/// (<c>AppState.Instancia.Analizador.CtrlGrupos</c>). Tiene dos secciones navegables:
///   1) Control Grupos    — pares (Grupos controlados, Fallos permitidos).
///   2) Control Conjuntos — pares (Conjuntos controlados, Fallos permitidos).
/// Cada sección es una lista de "controles" recorrible con Anterior/Siguiente y un
/// contador "actual/total". El control 0 es la base (grupos/conjuntos sueltos) y no se
/// muestra; la navegación empieza en 1.
///
/// Fidelidad legacy: se trabaja sobre COPIAS de las listas (ObtenCopiaControles*),
/// igual que el WinForms en InicializaDatos(), de modo que Cancelar descarta cambios.
/// </summary>
public partial class ControlGruposFrmViewModel : ObservableObject
{
    // Controlador real de la combinación actual (legacy: analizador.CtrlGrupos).
    private ControladorGrupos _ctrlGrupos = AppState.Instancia.Analizador.CtrlGrupos;

    // Copias de trabajo (legacy: campos controlesGrupos / controlesConjuntos).
    private List<ControlGrupos> _controlesGrupos = new();
    private List<ControlConjuntos> _controlesConjuntos = new();

    // Índices en pantalla (legacy: cgPantalla / cConjPantalla; arrancan en 1).
    private int _cgPantalla = 1;
    private int _cConjPantalla = 1;

    /// <summary>Acción de cierre/volver (la cablea la página con Frame.GoBack()).</summary>
    public Action? Volver { get; set; }

    // =========================================================================
    // Sección "Control Grupos"
    // =========================================================================

    // ===== Grupos controlados (txtGrupos) =====
    [ObservableProperty]
    private string _grupos = "";

    // ===== Fallos permitidos del control de grupos (txtFallos) =====
    [ObservableProperty]
    private string _fallosGrupos = "";

    // ===== Contador "actual/total" del control de grupos (lblControlNo) =====
    [ObservableProperty]
    private string _controlGruposNoTexto = "1/1";

    // =========================================================================
    // Sección "Control Conjuntos"
    // =========================================================================

    // ===== Conjuntos controlados (textConjuntos) =====
    [ObservableProperty]
    private string _conjuntos = "";

    // ===== Fallos permitidos del control de conjuntos (textFallosConj) =====
    [ObservableProperty]
    private string _fallosConjuntos = "";

    // ===== Contador "actual/total" del control de conjuntos (lblConjuntoNo) =====
    [ObservableProperty]
    private string _controlConjuntosNoTexto = "1/1";

    // ===== Estado / feedback =====
    [ObservableProperty]
    private string _estado = "Preparado";

    /// <summary>
    /// Carga las copias de trabajo y refresca la pantalla.
    /// Equivale a ControlGruposFrm.InicializaDatos() (ctor del form legacy).
    /// La página la llama en OnNavigatedTo para tomar el ControladorGrupos vigente.
    /// </summary>
    public void CargarDesdeMotor()
    {
        _ctrlGrupos = AppState.Instancia.Analizador.CtrlGrupos;
        _cgPantalla = 1;
        _cConjPantalla = 1;
        _controlesGrupos = ObtenCopiaControlesGrupos(_ctrlGrupos);
        _controlesConjuntos = ObtenCopiaControlesConjuntos(_ctrlGrupos);
        ActualizaDatosPantalla(_cgPantalla);
        ActualizaDatosPantallaConj(_cConjPantalla);
        Estado = "Preparado";
    }

    // ---- Copias de trabajo (legacy: ObtenCopiaControlesGrupos/Conjuntos) ----

    private static List<ControlGrupos> ObtenCopiaControlesGrupos(ControladorGrupos ctrlGrupos)
    {
        var copia = new List<ControlGrupos>();
        foreach (ControlGrupos cg in ctrlGrupos.ControlesGrupos)
        {
            var cgCopia = new ControlGrupos();
            cgCopia.PonerGruposControlados(cg.ObtenGruposControlados());
            cgCopia.PonerFallosPermitidos(cg.ObtenFallosPermitidos());
            cgCopia.CtrlGrupos = cg.CtrlGrupos;
            cgCopia.UsaControlGrupos = cg.UsaControlGrupos;
            copia.Add(cgCopia);
        }
        return copia;
    }

    private static List<ControlConjuntos> ObtenCopiaControlesConjuntos(ControladorGrupos ctrlGrupos)
    {
        var copia = new List<ControlConjuntos>();
        foreach (ControlConjuntos cConj in ctrlGrupos.ControlesConjuntos)
        {
            var cConjCopia = new ControlConjuntos();
            cConjCopia.PonerCtrlGruposControlados(cConj.ObtenCtrlGruposControladosStr());
            cConjCopia.PonerFallosPermitidos(cConj.ObtenFallosPermitidosStr());
            copia.Add(cConjCopia);
        }
        return copia;
    }

    // =========================================================================
    // Sección Grupos — pantalla / navegación (legacy region controlGrupos)
    // =========================================================================

    private void ActualizaDatosPantalla(int noCG)
    {
        if (_controlesGrupos.Count > 1)
        {
            ControlGrupos cg = _controlesGrupos[noCG];
            Grupos = cg.ObtenGruposControlados();
            FallosGrupos = cg.ObtenFallosPermitidos();
            ControlGruposNoTexto = noCG + "/" + (_controlesGrupos.Count - 1);
        }
        else
        {
            Grupos = "";
            FallosGrupos = "";
            ControlGruposNoTexto = "1/1";
        }
    }

    private bool TieneDatosControl() =>
        !string.IsNullOrEmpty(Grupos) && !string.IsNullOrEmpty(FallosGrupos);

    private void GuardaDatosCG(ControlGrupos cg)
    {
        cg.PonerGruposControlados(Grupos);
        cg.PonerFallosPermitidos(FallosGrupos);
    }

    private void GuardarCGActual()
    {
        if (_cgPantalla < _controlesGrupos.Count)
        {
            if (TieneDatosControl())
            {
                GuardaDatosCG(_controlesGrupos[_cgPantalla]);
            }
        }
        else if (TieneDatosControl())
        {
            // existen datos en pantalla que necesitan ponerse en nuevo control
            var cg = new ControlGrupos { CtrlGrupos = _ctrlGrupos };
            _controlesGrupos.Add(cg);
            GuardaDatosCG(cg);
        }
    }

    private void CambiaCGSelecionado(int noCG)
    {
        GuardarCGActual();
        _cgPantalla = noCG;

        // crear CG si no existe
        if (_controlesGrupos.Count < noCG + 1)
        {
            _controlesGrupos.Add(new ControlGrupos { CtrlGrupos = _ctrlGrupos });
        }

        ActualizaDatosPantalla(noCG);
    }

    private bool NecesitaBorrarUltimaCG()
    {
        // controlgrupo 0 es la base. No borrar.
        if (_controlesGrupos.Count > 1)
        {
            ControlGrupos cg = _controlesGrupos[^1];
            if (cg.ObtenGruposControlados() == "" || cg.ObtenFallosPermitidos() == "")
            {
                return true;
            }
        }
        return false;
    }

    private void GuardarGruposLibres()
    {
        int noGruposTotal = _ctrlGrupos.GruposPartidos.Count;
        string strGruposLibres = "0"; // grupo 0 siempre libre (boleto base)

        for (int noGrupo = 1; noGrupo < noGruposTotal; noGrupo++)
        {
            bool contieneGrupo = false;
            for (int i = 1; i < _controlesGrupos.Count; i++)
            {
                if (_controlesGrupos[i].ContieneGrupo(noGrupo))
                {
                    contieneGrupo = true;
                    break;
                }
            }
            if (!contieneGrupo)
            {
                strGruposLibres += "," + noGrupo;
            }
        }

        ControlGrupos cgBase = _controlesGrupos[0];
        cgBase.PonerGruposControlados(strGruposLibres);
        cgBase.UsaControlGrupos = false;
    }

    private void GuardarDatos()
    {
        GuardarCGActual();
        if (_controlesGrupos.Count > 0 && NecesitaBorrarUltimaCG())
        {
            _controlesGrupos.RemoveAt(_controlesGrupos.Count - 1);
        }
        GuardarGruposLibres();
        _ctrlGrupos.ControlesGrupos = _controlesGrupos;
    }

    // =========================================================================
    // Sección Conjuntos — pantalla / navegación (legacy region controles Conjuntos)
    // =========================================================================

    private void ActualizaDatosPantallaConj(int noConj)
    {
        if (_controlesConjuntos.Count > 1)
        {
            ControlConjuntos cConj = _controlesConjuntos[noConj];
            Conjuntos = cConj.ObtenCtrlGruposControladosStr();
            FallosConjuntos = cConj.ObtenFallosPermitidosStr();
            ControlConjuntosNoTexto = noConj + "/" + (_controlesConjuntos.Count - 1);
        }
        else
        {
            Conjuntos = "";
            FallosConjuntos = "";
            ControlConjuntosNoTexto = "1/1";
        }
    }

    private bool TieneDatosControlConj() =>
        !string.IsNullOrEmpty(Conjuntos) && !string.IsNullOrEmpty(FallosConjuntos);

    private void GuardaDatosCConj(ControlConjuntos cConj)
    {
        cConj.PonerCtrlGruposControlados(Conjuntos);
        cConj.PonerFallosPermitidos(FallosConjuntos);
    }

    private void GuardarCConjActual()
    {
        if (_cConjPantalla < _controlesConjuntos.Count)
        {
            if (TieneDatosControlConj())
            {
                GuardaDatosCConj(_controlesConjuntos[_cConjPantalla]);
            }
        }
        else if (TieneDatosControlConj())
        {
            var cConj = new ControlConjuntos();
            _controlesConjuntos.Add(cConj);
            GuardaDatosCConj(cConj);
        }
    }

    private void CambiaCConjSelecionado(int noConj)
    {
        GuardarCConjActual();
        _cConjPantalla = noConj;

        if (_controlesConjuntos.Count < noConj + 1)
        {
            _controlesConjuntos.Add(new ControlConjuntos());
        }

        ActualizaDatosPantallaConj(noConj);
    }

    private bool NecesitaBorrarUltimaCConj()
    {
        if (_controlesConjuntos.Count > 1)
        {
            ControlConjuntos cConj = _controlesConjuntos[^1];
            if (cConj.ObtenCtrlGruposControladosStr() == "" || cConj.ObtenFallosPermitidosStr() == "")
            {
                return true;
            }
        }
        return false;
    }

    private void GuardarGruposLibresCConj()
    {
        int noConjuntosTotal = _ctrlGrupos.ControlesGrupos.Count;
        string strConjuntosLibres = "0"; // conjunto 0 siempre libre

        for (int noConjunto = 1; noConjunto < noConjuntosTotal; noConjunto++)
        {
            bool contieneConjunto = false;
            for (int i = 1; i < _controlesConjuntos.Count; i++)
            {
                if (_controlesConjuntos[i].ContieneConjunto(noConjunto))
                {
                    contieneConjunto = true;
                    break;
                }
            }
            if (!contieneConjunto)
            {
                strConjuntosLibres += "," + noConjunto;
            }
        }

        ControlConjuntos cConjBase = _controlesConjuntos[0];
        cConjBase.PonerCtrlGruposControlados(strConjuntosLibres);
        cConjBase.PonerFallosPermitidos("0");
    }

    private void GuardarDatosCConj()
    {
        GuardarCConjActual();
        if (_controlesConjuntos.Count > 0 && NecesitaBorrarUltimaCConj())
        {
            _controlesConjuntos.RemoveAt(_controlesConjuntos.Count - 1);
        }
        GuardarGruposLibresCConj();
        _ctrlGrupos.ControlesConjuntos = _controlesConjuntos;
    }

    // =========================================================================
    // Comandos sección Grupos
    // =========================================================================

    /// <summary>Equivale a <c>BtnPrevClick</c>: CambiaCGSelecionado(cgPantalla - 1).</summary>
    [RelayCommand]
    private void GruposAnterior()
    {
        if (_cgPantalla <= 1) return; // botón "atrás" inhabilitado en la primera columna
        CambiaCGSelecionado(_cgPantalla - 1);
    }

    /// <summary>Equivale a <c>BtnNextClick</c>: avanza solo si el control actual tiene datos.</summary>
    [RelayCommand]
    private void GruposSiguiente()
    {
        if (TieneDatosControl())
        {
            CambiaCGSelecionado(_cgPantalla + 1);
        }
    }

    /// <summary>Equivale a <c>btnEliminarControl_Click</c>: borra el control actual (nunca el base).</summary>
    [RelayCommand]
    private void GruposEliminarActual()
    {
        if (_cgPantalla == 1)
        {
            // solo borrar si la CG ya está guardada en memoria
            if (_controlesGrupos.Count > 1)
            {
                _controlesGrupos.RemoveAt(_cgPantalla);
            }
        }
        else
        {
            _controlesGrupos.RemoveAt(_cgPantalla);
            _cgPantalla -= 1;
        }
        ActualizaDatosPantalla(_cgPantalla);
    }

    // =========================================================================
    // Comandos sección Conjuntos
    // =========================================================================

    /// <summary>Equivale a <c>btnPrevConj_Click</c>: CambiaCConjSelecionado(cConjPantalla - 1).</summary>
    [RelayCommand]
    private void ConjuntosAnterior()
    {
        if (_cConjPantalla <= 1) return;
        CambiaCConjSelecionado(_cConjPantalla - 1);
    }

    /// <summary>Equivale a <c>btnNextConj_Click</c>: avanza solo si el control actual tiene datos.</summary>
    [RelayCommand]
    private void ConjuntosSiguiente()
    {
        if (TieneDatosControlConj())
        {
            CambiaCConjSelecionado(_cConjPantalla + 1);
        }
    }

    /// <summary>Equivale a <c>btnEliminarCtrlConjunto_Click</c>: borra el control actual (nunca el base).</summary>
    [RelayCommand]
    private void ConjuntosEliminarActual()
    {
        if (_cConjPantalla == 1)
        {
            if (_controlesConjuntos.Count > 1)
            {
                _controlesConjuntos.RemoveAt(_cConjPantalla);
            }
        }
        else
        {
            _controlesConjuntos.RemoveAt(_cConjPantalla);
            _cConjPantalla -= 1;
        }
        ActualizaDatosPantallaConj(_cConjPantalla);
    }

    // =========================================================================
    // Acciones globales (OK / Cancelar)
    // =========================================================================

    /// <summary>Equivale a <c>BtnOKClick</c>: GuardarDatos() + GuardarDatosCConj() + Close().</summary>
    [RelayCommand]
    private void Aceptar()
    {
        GuardarDatos();
        GuardarDatosCConj();
        AppState.Instancia.NotificarCambio();
        Estado = "Guardado";
        Volver?.Invoke();
    }

    /// <summary>Equivale a <c>BtnCancelarClick</c>: descarta cambios y cierra (las copias se descartan).</summary>
    [RelayCommand]
    private void Cancelar()
    {
        Estado = "Cancelado";
        Volver?.Invoke();
    }
}
