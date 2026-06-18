using System.Data;
using System.IO;
using Free1X2.Escrutinio;
using Free1X2.Reduccion;
using Xunit;

namespace Free1X2.Domain.Tests
{
    /// <summary>
    /// Integración del Escrutador y de un reductor (IReduccion) usando un fichero de
    /// columnas REALES (fixture cols_a.txt, 10 ganadoras 2008-09). A diferencia de
    /// FlujosClaveTests (que genera el universo con Analizador), aquí la entrada es un
    /// fichero de columnas reales arbitrarias, como las que cargan las pantallas WinUI.
    /// </summary>
    public class EscrutinioReduccionFixtureTests
    {
        private static DataSet CrearDataSetResultados()
        {
            var ds = new DataSet();
            var t = new DataTable("Resultados");
            t.Columns.Add("Seleccionado", typeof(bool));
            t.Columns.Add("LineaID", typeof(int));
            t.Columns.Add("Columna", typeof(string));
            t.Columns.Add("Archivo", typeof(string));
            for (int i = 0; i <= 14; i++) t.Columns.Add("P" + i, typeof(int));
            t.Columns.Add("Ac. Totales", typeof(string));
            ds.Tables.Add(t);
            return ds;
        }

        [Fact]
        public void Escrutar_ColumnasReales_DistribucionDeAciertosEsLaEsperada()
        {
            string fichero = FixturePaths.Ruta(FixturePaths.ColsA);
            string ganadora = File.ReadAllLines(FixturePaths.Ruta(FixturePaths.Ganadora))[0].Trim();

            var ds = CrearDataSetResultados();
            var escrutador = new Escrutador(new[] { 14, 13, 12, 11, 10 })
            {
                ArchivoColumnas = fichero,
            };
            escrutador.EscrutaCombConColumna(ganadora, ds, Path.GetFileName(fichero));

            int[] premios = escrutador.PremiosTotales;

            // Distribución golden-master calculada a mano sobre los datos reales:
            //   14->1, 8->1, 7->2, 6->2, 5->2, 4->2  (suma = 10 columnas)
            Assert.Equal(1, premios[14]); // la ganadora está en el fichero
            Assert.Equal(1, premios[8]);
            Assert.Equal(2, premios[7]);
            Assert.Equal(2, premios[6]);
            Assert.Equal(2, premios[5]);
            Assert.Equal(2, premios[4]);

            int total = 0;
            for (int i = 0; i <= 14; i++) total += premios[i];
            Assert.Equal(10, total); // se puntúan exactamente las 10 columnas del fichero
        }

        [Fact]
        public void Reducir_JDC_SobreColumnasReales_ProduceSubconjuntoNoMayorQueLaEntrada()
        {
            string entrada = FixturePaths.Ruta(FixturePaths.ColsA);
            string salida = Path.Combine(Path.GetTempPath(),
                "free1x2_redu_fixture_" + System.Guid.NewGuid().ToString("N") + ".txt");
            try
            {
                IReduccion reductor = new JDC(false);
                reductor.ComienzaReduccion(entrada, salida, 13, 0, 100);

                Assert.Equal(10, reductor.NoColumnasIniciales);
                Assert.True(reductor.NoColumnasFinales > 0, "la reducción no produjo columnas");
                Assert.True(reductor.NoColumnasFinales <= reductor.NoColumnasIniciales,
                    "una reducción no puede tener más columnas que la entrada");

                Assert.True(File.Exists(salida), "no se escribió el fichero de salida");
                int lineas = File.ReadAllLines(salida).Length;
                Assert.Equal(reductor.NoColumnasFinales, lineas);
            }
            finally { if (File.Exists(salida)) File.Delete(salida); }
        }
    }
}
