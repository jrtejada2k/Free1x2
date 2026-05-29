namespace Free1X2.Abstractions
{
    /// <summary>
    /// Abstrae las rutas de la aplicación (antes Application.StartupPath), para que
    /// el dominio no dependa de System.Windows.Forms. La UI inyecta la implementación real.
    /// </summary>
    public interface IAppPaths
    {
        /// <summary>Carpeta base de la aplicación (equivalente a Application.StartupPath).</summary>
        string StartupPath { get; }
    }

    /// <summary>
    /// Implementación por defecto basada en el directorio del ejecutable.
    /// Sirve para tests y headless; la UI puede sustituirla si lo necesita.
    /// </summary>
    public sealed class AppPaths : IAppPaths
    {
        public AppPaths(string startupPath) { StartupPath = startupPath; }
        public AppPaths() : this(System.AppContext.BaseDirectory) { }
        public string StartupPath { get; }
    }
}
