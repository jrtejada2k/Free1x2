using System.Collections;
using System.IO;
using Free1X2.Utils;
using Xunit;

namespace Free1X2.Domain.Tests
{
    /// <summary>
    /// Round-trip de <see cref="CompresorZip"/> (portado desde el WinForms original,
    /// formato *.z3q sobre ICSharpCode.SharpZipLib): un BitArray de columnas se comprime
    /// a un .z3q y se descomprime de vuelta, comprobando que los bits puestos sobreviven.
    ///
    /// Descomprimir() devuelve siempre un BitArray de tamaño 4.782.969 (= 3^14, las
    /// columnas de 14 partidos), por lo que las posiciones de prueba están dentro de ese
    /// rango y se contrastan contra ese tamaño fijo.
    /// </summary>
    public class CompresorZipTests
    {
        private const int TamColumnas = 4782969; // 3^14

        [Fact]
        public void ComprimirDescomprimir_PreservaLosBitsActivos()
        {
            // Algunas posiciones representativas (extremos + interior).
            int[] posiciones = { 0, 1, 7, 8, 9, 1234, 100000, 2391484, TamColumnas - 1 };

            var origen = new BitArray(TamColumnas, false);
            foreach (int p in posiciones)
            {
                origen[p] = true;
            }

            string ruta = Path.Combine(Path.GetTempPath(), "free1x2_compresor_test_" + System.Guid.NewGuid().ToString("N") + ".z3q");
            try
            {
                long tamDatos = CompresorZip.Comprimir(origen, ruta, ruta, origen.Length, 6);

                Assert.True(File.Exists(ruta));
                Assert.True(new FileInfo(ruta).Length > 0);
                // (tamano / 8) + 1 bytes empaquetados (réplica del original).
                Assert.Equal((origen.Length / 8) + 1, tamDatos);

                BitArray salida = CompresorZip.Descomprimir(ruta);
                Assert.Equal(TamColumnas, salida.Length);

                var esperados = new System.Collections.Generic.HashSet<int>(posiciones);
                for (int i = 0; i < TamColumnas; i++)
                {
                    Assert.Equal(esperados.Contains(i), salida[i]);
                }
            }
            finally
            {
                if (File.Exists(ruta))
                {
                    File.Delete(ruta);
                }
            }
        }
    }
}
