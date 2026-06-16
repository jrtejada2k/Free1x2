using System;

namespace Free1X2.Abstractions
{
    /// <summary>
    /// Punto de bombeo de mensajes de UI desacoplado. El dominio llama
    /// <see cref="Pump"/> en bucles largos (antes <c>Application.DoEvents()</c>).
    /// La capa de UI asigna <see cref="Pump"/> = Application.DoEvents al arrancar;
    /// por defecto es no-op (headless/tests/WinUI).
    /// </summary>
    public static class UiPump
    {
        public static Action Pump = static () => { };
    }

    /// <summary>
    /// Diálogos de usuario desacoplados. El dominio reporta avisos sin depender de
    /// <c>System.Windows.Forms.MessageBox</c>. La UI asigna las implementaciones;
    /// por defecto no-op (no se rompe en headless/tests/WinUI).
    /// </summary>
    public static class UserDialogs
    {
        public static Action<string> ShowError = static _ => { };
        public static Action<string> ShowInfo  = static _ => { };
    }

    /// <summary>
    /// Hook para que el dominio solicite a la UI mostrar el visor de análisis de
    /// columnas, sin referenciar el Form. La UI asigna la implementación; el
    /// dominio invoca con (contenedorAnalisisGlobal, grupo). Por defecto no-op.
    /// </summary>
    public static class AnalisisUi
    {
        public static Action<object, object> MostrarVisor = static (_, __) => { };
    }

    /// <summary>
    /// Portapapeles desacoplado. El dominio formatea/parsea texto sin depender de
    /// <c>System.Windows.Forms.Clipboard</c>. La UI asigna las implementaciones
    /// (WinForms Clipboard / WinUI DataPackage); por defecto leer devuelve vacío y
    /// escribir es no-op (headless/tests/WinUI sin cablear).
    /// </summary>
    public static class Clipboard
    {
        public static Func<string> Read  = static () => string.Empty;
        public static Action<string> Write = static _ => { };
    }
}
