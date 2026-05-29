namespace Free1X2.Abstractions
{
    /// <summary>
    /// Permite que la lógica de dominio ceda el control al hilo de UI durante
    /// operaciones largas, sin depender de System.Windows.Forms (Application.DoEvents).
    /// La capa de UI provee una implementación; el dominio usa NullProgressNotifier por defecto.
    /// </summary>
    public interface IProgressNotifier
    {
        /// <summary>Equivalente desacoplado de Application.DoEvents().</summary>
        void AllowUIToProcess();

        /// <summary>Reporta progreso 0..100 (opcional).</summary>
        void Report(int percent, string message = null);
    }

    /// <summary>Implementación nula: no hace nada. Usada cuando no hay UI (tests, headless).</summary>
    public sealed class NullProgressNotifier : IProgressNotifier
    {
        public static readonly NullProgressNotifier Instance = new NullProgressNotifier();
        public void AllowUIToProcess() { }
        public void Report(int percent, string message = null) { }
    }
}
