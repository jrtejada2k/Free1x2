using System.IO;

namespace Free1X2.Domain.Tests
{
    /// <summary>
    /// Localiza las fixtures de datos reales (copiadas a la carpeta de salida del
    /// proyecto de pruebas vía CopyToOutputDirectory). Mantiene los tests PORTABLES:
    /// nunca se referencia la ruta original de Downloads, solo AppContext.BaseDirectory.
    /// </summary>
    internal static class FixturePaths
    {
        private static readonly string Raiz =
            Path.Combine(System.AppContext.BaseDirectory, "Fixtures");

        internal static string Ruta(string nombre) => Path.Combine(Raiz, nombre);

        /// <summary>10 columnas reales distintas (ganadoras 2008-09, líneas 1-10).</summary>
        internal const string ColsA = "cols_a.txt";

        /// <summary>10 columnas reales; comparten 5 con A (líneas 6-10) + 5 nuevas.</summary>
        internal const string ColsB = "cols_b.txt";

        /// <summary>10 líneas con duplicados; 6 columnas distintas.</summary>
        internal const string ColsConRepetidas = "cols_con_repetidas.txt";

        /// <summary>Una sola columna ganadora; coincide con la 1ª de cols_a.</summary>
        internal const string Ganadora = "ganadora.txt";
    }
}
