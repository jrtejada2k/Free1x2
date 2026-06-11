using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.Input;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// ViewModel para la pantalla "Control de Grupos".
/// Replica los campos de entrada del WinForms <c>ControlGruposFrm</c>, que tiene
/// dos secciones navegables independientes:
///   1) Control Grupos    — pares (Grupos controlados, Fallos permitidos).
///   2) Control Conjuntos — pares (Conjuntos controlados, Fallos permitidos).
/// Cada sección es una lista de "controles" recorrible con Anterior/Siguiente y
/// un contador "actual/total". El control 0 es la base (grupos/conjuntos sueltos)
/// y no se muestra; la navegación empieza en 1. La lógica de copia, guardado y
/// cálculo de grupos libres queda como TODO hasta portar el dominio.
/// </summary>
public partial class ControlGruposFrmViewModel : ObservableObject
{
    // =========================================================================
    // Sección "Control Grupos" (groupBox)
    // =========================================================================

    // ===== Grupos controlados (txtGrupos) =====
    [ObservableProperty]
    private string _grupos = "";

    // ===== Fallos permitidos del control de grupos (txtFallos) =====
    [ObservableProperty]
    private string _fallosGrupos = "";

    // ===== Contador "actual/total" del control de grupos (lblControlNo) =====
    // El WinForms arranca en "1/1" cuando solo existe el control base.
    [ObservableProperty]
    private string _controlGruposNoTexto = "1/1";

    // =========================================================================
    // Sección "Control Conjuntos" (groupBox1)
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

    // =========================================================================
    // Comandos sección Grupos
    // =========================================================================

    /// <summary>
    /// Equivale a <c>BtnPrevClick</c>: navega al control de grupos anterior
    /// (CambiaCGSelecionado(cgPantalla - 1)).
    /// </summary>
    [RelayCommand]
    private void GruposAnterior()
    {
        // TODO(dominio): portar CambiaCGSelecionado / GuardarCGActual de
        //   Free1X2.UI.Filtros.ControlGruposFrm. Antes de cambiar de control,
        //   el WinForms guarda los datos en pantalla, decrementa cgPantalla,
        //   deshabilita "Anterior" si cgPantalla == 1 y refresca con
        //   ActualizaDatosPantalla(cgPantalla). Trabaja sobre la lista
        //   List<ControlGrupos> (MotorCalculo).
        Estado = "Navegar grupos: pendiente de portar dominio";
    }

    /// <summary>
    /// Equivale a <c>BtnNextClick</c>: navega al siguiente control de grupos solo
    /// si el actual contiene datos (TieneDatosControl()).
    /// </summary>
    [RelayCommand]
    private void GruposSiguiente()
    {
        // TODO(dominio): portar BtnNextClick / CambiaCGSelecionado. El WinForms
        //   solo avanza si txtGrupos y txtFallos no están vacíos; si el control
        //   siguiente no existe lo crea (new ControlGrupos { CtrlGrupos = ... }).
        Estado = "Navegar grupos: pendiente de portar dominio";
    }

    /// <summary>
    /// Equivale a <c>btnEliminarControl_Click</c>: elimina el control de grupos
    /// actual (BorrarCG), nunca el control base (índice 0).
    /// </summary>
    [RelayCommand]
    private void GruposEliminarActual()
    {
        // TODO(dominio): portar btnEliminarControl_Click. Solo borra si la
        //   columna ya está guardada en memoria; el control 0 (base) nunca se
        //   borra. Ajusta cgPantalla y el estado del botón "Anterior".
        Estado = "Eliminar control de grupos: pendiente de portar dominio";
    }

    // =========================================================================
    // Comandos sección Conjuntos
    // =========================================================================

    /// <summary>
    /// Equivale a <c>btnPrevConj_Click</c>: navega al control de conjuntos anterior
    /// (CambiaCConjSelecionado(cConjPantalla - 1)).
    /// </summary>
    [RelayCommand]
    private void ConjuntosAnterior()
    {
        // TODO(dominio): portar CambiaCConjSelecionado / GuardarCConjActual sobre
        //   List<ControlConjuntos> (MotorCalculo).
        Estado = "Navegar conjuntos: pendiente de portar dominio";
    }

    /// <summary>
    /// Equivale a <c>btnNextConj_Click</c>: navega al siguiente control de conjuntos
    /// solo si el actual contiene datos (TieneDatosControlConj()).
    /// </summary>
    [RelayCommand]
    private void ConjuntosSiguiente()
    {
        // TODO(dominio): portar btnNextConj_Click / CambiaCConjSelecionado.
        Estado = "Navegar conjuntos: pendiente de portar dominio";
    }

    /// <summary>
    /// Equivale a <c>btnEliminarCtrlConjunto_Click</c>: elimina el control de
    /// conjuntos actual (BorrarCConj), nunca el base (índice 0).
    /// </summary>
    [RelayCommand]
    private void ConjuntosEliminarActual()
    {
        // TODO(dominio): portar btnEliminarCtrlConjunto_Click.
        Estado = "Eliminar control de conjuntos: pendiente de portar dominio";
    }

    // =========================================================================
    // Acciones globales (OK / Cancelar)
    // =========================================================================

    /// <summary>
    /// Equivale a <c>BtnOKClick</c>: guarda ambas secciones y cierra el formulario.
    /// </summary>
    [RelayCommand]
    private void Aceptar()
    {
        // TODO(dominio): portar GuardarDatos() + GuardarDatosCConj() de
        //   Free1X2.UI.Filtros.ControlGruposFrm:
        //     - GuardarCGActual / GuardarCConjActual (volcar pantalla al control).
        //     - Borrar el último control si quedó vacío (NecesitaBorrarUltimaCG/CConj).
        //     - GuardarGruposLibres / GuardarGruposLibresCConj (recolectar los
        //       grupos/conjuntos no controlados en el control base [0]).
        //     - Asignar las listas resultantes a
        //       ctrlGrupos.ControlesGrupos / ctrlGrupos.ControlesConjuntos
        //       (ControladorGrupos de MotorCalculo).
        //   El WinForms cerraba la ventana tras guardar (Close()).
        Estado = "Guardado (pendiente de portar dominio)";
    }

    /// <summary>
    /// Equivale a <c>BtnCancelarClick</c>: descarta los cambios y cierra.
    /// </summary>
    [RelayCommand]
    private void Cancelar()
    {
        // TODO(dominio): portar BtnCancelarClick (Close() sin guardar). En WinUI
        //   esto lo gestionará la navegación del shell.
        Estado = "Cancelado (pendiente de portar dominio)";
    }
}
