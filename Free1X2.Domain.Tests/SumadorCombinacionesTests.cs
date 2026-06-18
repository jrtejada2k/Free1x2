using System;
using System.IO;
using System.Linq;
using Free1X2;
using Free1X2.Utils;
using Xunit;

namespace Free1X2.Domain.Tests
{
    /// <summary>
    /// Golden-master del álgebra de combinaciones (<see cref="SumadorCombinaciones"/>),
    /// el motor detrás de la pantalla "Operaciones con combinaciones" de WinUI. Ejecuta
    /// las 4 operaciones reales sobre dos ficheros de columnas reales y comprueba el
    /// fichero de salida (recuento de columnas distintas y la relación entre conjuntos).
    ///
    /// Fixtures (columnas reales 2008-09):
    ///   A = 10 columnas distintas; B = 10 columnas distintas que comparten 5 con A.
    ///   => A∪B = 15 ; A∩B = 5 ; A\B = 5 ; dedup(repetidas) = 6 (de 10 líneas).
    /// </summary>
    public class SumadorCombinacionesTests
    {
        private static string Salida() =>
            Path.Combine(Path.GetTempPath(), "free1x2_sumador_" + Guid.NewGuid().ToString("N") + ".txt");

        private static int ContarColumnasDistintas(string ruta)
        {
            return File.ReadAllLines(ruta)
                       .Select(l => l.Trim())
                       .Where(l => l.Length > 0)
                       .Distinct()
                       .Count();
        }

        private static SumadorCombinaciones Nuevo(string final)
        {
            return new SumadorCombinaciones(VariablesGlobales.NumeroPartidos)
            {
                ArchivoCols1 = FixturePaths.Ruta(FixturePaths.ColsA),
                ArchivoCols2 = FixturePaths.Ruta(FixturePaths.ColsB),
                ArchivoColsFinal = final,
            };
        }

        [Fact]
        public void SumaEliminaRepetidas_ProduceLaUnion_15Columnas()
        {
            string final = Salida();
            try
            {
                var s = Nuevo(final);
                s.Calcula(AlgebraCombTipo.SumaEliminaRepetidas);

                Assert.True(File.Exists(final));
                Assert.Equal(15, ContarColumnasDistintas(final)); // |A ∪ B|
            }
            finally { if (File.Exists(final)) File.Delete(final); }
        }

        [Fact]
        public void SumaSoloComunes_ProduceLaInterseccion_5Columnas()
        {
            string final = Salida();
            try
            {
                var s = Nuevo(final);
                s.Calcula(AlgebraCombTipo.SumaSoloComunes);

                Assert.True(File.Exists(final));
                Assert.Equal(5, ContarColumnasDistintas(final)); // |A ∩ B|
            }
            finally { if (File.Exists(final)) File.Delete(final); }
        }

        [Fact]
        public void RestaSegunda_ProduceLaDiferencia_5Columnas()
        {
            string final = Salida();
            try
            {
                var s = Nuevo(final);
                s.Calcula(AlgebraCombTipo.RestaSegunda);

                Assert.True(File.Exists(final));
                Assert.Equal(5, ContarColumnasDistintas(final)); // |A \ B|
            }
            finally { if (File.Exists(final)) File.Delete(final); }
        }

        [Fact]
        public void EliminaRepetidas_DeduplicaFichero_De10LineasA6Distintas()
        {
            string final = Salida();
            try
            {
                var s = new SumadorCombinaciones(VariablesGlobales.NumeroPartidos)
                {
                    ArchivoCols1 = FixturePaths.Ruta(FixturePaths.ColsConRepetidas),
                    ArchivoColsFinal = final,
                };
                s.Calcula(AlgebraCombTipo.EliminaRepetidas);

                Assert.True(File.Exists(final));
                int lineasSalida = File.ReadAllLines(final).Count(l => l.Trim().Length > 0);
                // dedup: 6 distintas; la salida no contiene duplicados.
                Assert.Equal(6, lineasSalida);
                Assert.Equal(6, ContarColumnasDistintas(final));
            }
            finally { if (File.Exists(final)) File.Delete(final); }
        }

        [Fact]
        public void Relaciones_DeConjuntos_SonCoherentes()
        {
            // |A∪B| = |A| + |B| - |A∩B|  y  |A\B| = |A| - |A∩B|.
            // Verificado ejecutando el motor real, no recalculando aparte.
            string fUnion = Salida(), fInter = Salida(), fResta = Salida();
            try
            {
                Nuevo(fUnion).Calcula(AlgebraCombTipo.SumaEliminaRepetidas);
                Nuevo(fInter).Calcula(AlgebraCombTipo.SumaSoloComunes);
                Nuevo(fResta).Calcula(AlgebraCombTipo.RestaSegunda);

                int union = ContarColumnasDistintas(fUnion);
                int inter = ContarColumnasDistintas(fInter);
                int resta = ContarColumnasDistintas(fResta);

                int a = ContarColumnasDistintas(FixturePaths.Ruta(FixturePaths.ColsA));
                int b = ContarColumnasDistintas(FixturePaths.Ruta(FixturePaths.ColsB));

                Assert.Equal(a + b - inter, union);
                Assert.Equal(a - inter, resta);
            }
            finally
            {
                foreach (var f in new[] { fUnion, fInter, fResta })
                    if (File.Exists(f)) File.Delete(f);
            }
        }
    }
}
