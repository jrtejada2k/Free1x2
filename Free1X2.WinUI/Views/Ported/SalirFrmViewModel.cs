using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// ViewModel para el diálogo de confirmación de salida (legacy: SalirFrm).
/// El form original preguntaba "¿Salir del programa?" con un checkbox para no
/// volver a mostrar la advertencia, y botones "Sí" / "Cancelar".
/// </summary>
public partial class SalirFrmViewModel : ObservableObject
{
    /// <summary>
    /// Equivalente al CheckBox legacy "chbNoMostrar" ("No mostrar este mensaje de nuevo").
    /// Si está activado, el usuario no quiere ver de nuevo la advertencia de salida.
    /// </summary>
    [ObservableProperty]
    private bool _noMostrarDeNuevo;

    /// <summary>
    /// Indica si el usuario confirmó la salida (legacy: campo público SalirFrm.exit).
    /// </summary>
    [ObservableProperty]
    private bool _salirConfirmado;

    /// <summary>
    /// Confirma la salida del programa (legacy: btnOK_Click, exit = true).
    /// </summary>
    [RelayCommand]
    private void ConfirmarSalida()
    {
        SalirConfirmado = true;
        GuardarPreferenciaAdvertencia();

        // TODO[dominio]: cerrar la aplicación tras confirmar.
        //   Legacy: SalirFrm.btnOK_Click ponía exit = true y this.Close();
        //   el MainForm consumía SalirFrm.exit para terminar la app.
        //   En WinUI decidir aquí Application.Current.Exit() o cerrar la ventana host.
    }

    /// <summary>
    /// Cancela la salida (legacy: btnCancel_Click, exit = false).
    /// </summary>
    [RelayCommand]
    private void CancelarSalida()
    {
        SalirConfirmado = false;
        GuardarPreferenciaAdvertencia();

        // TODO[dominio]: cerrar el diálogo sin salir.
        //   Legacy: SalirFrm.btnCancel_Click ponía exit = false y this.Close();
        //   En navegación WinUI, invocar Frame.GoBack() o cerrar el host contenedor.
    }

    /// <summary>
    /// Persiste si debe seguir mostrándose la advertencia de salida.
    /// </summary>
    private void GuardarPreferenciaAdvertencia()
    {
        // TODO[dominio]: guardar la preferencia de advertencia de salida.
        //   Legacy: new AConfiguracion(Application.StartupPath)
        //           .GuardarConfiguracionAdvertenciaSalir(!chbNoMostrar.Checked);
        //   Es decir, se guarda "mostrar = !NoMostrarDeNuevo".
        //   El dominio (Free1X2.EntradaSalida.AConfiguracion) aún no está migrado.
    }
}
