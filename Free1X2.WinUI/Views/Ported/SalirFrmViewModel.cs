using System;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Free1X2.EntradaSalida;

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
        // Legacy: SalirFrm.btnOK_Click -> exit = true; guardar preferencia; this.Close();
        SalirConfirmado = true;
        GuardarPreferenciaAdvertencia();

        // Legacy: MainForm consumía SalirFrm.exit para terminar la app.
        Microsoft.UI.Xaml.Application.Current.Exit();
    }

    /// <summary>
    /// Cancela la salida (legacy: btnCancel_Click, exit = false).
    /// </summary>
    [RelayCommand]
    private void CancelarSalida()
    {
        // Legacy: SalirFrm.btnCancel_Click -> exit = false; guardar preferencia; this.Close().
        SalirConfirmado = false;
        GuardarPreferenciaAdvertencia();
        // El cierre del diálogo/navegación lo gestiona la página host (code-behind / Frame).
    }

    /// <summary>
    /// Persiste si debe seguir mostrándose la advertencia de salida.
    /// Legacy: AConfiguracion.GuardarConfiguracionAdvertenciaSalir(!chbNoMostrar.Checked),
    /// es decir se guarda "mostrar = !NoMostrarDeNuevo".
    /// </summary>
    private void GuardarPreferenciaAdvertencia()
    {
        try
        {
            // Legacy: new AConfiguracion(Application.StartupPath). En WinUI el directorio
            // de inicio equivale a AppContext.BaseDirectory, que es el ctor por defecto.
            var aConf = new AConfiguracion();
            aConf.GuardarConfiguracionAdvertenciaSalir(!NoMostrarDeNuevo);
        }
        catch (Exception ex)
        {
            // El legacy no protegía esta llamada, pero en WinUI evitamos tumbar la app
            // si parametros.free1x2 no está presente; se informa por el servicio de errores.
            Services.AppServices.MostrarError(
                $"No se pudo guardar la preferencia de salida: {ex.Message}");
        }
    }
}
