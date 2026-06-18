using System.IO;
using Free1X2.Utils;
using Xunit;

namespace Free1X2.Domain.Tests
{
    /// <summary>
    /// Golden-master de <see cref="UtilColumnas"/>: empaquetado de una columna 1/X/2
    /// en un long (3 bits por partido: 1->100b=4, X->010b=2, 2->001b=1) y su vuelta.
    /// Además del round-trip string<->long, se verifica el invariante de dominio
    /// coincidencias + diferencias == NumeroPartidos contra columnas reales.
    /// </summary>
    public class UtilColumnasTests
    {
        [Theory]
        [InlineData("11111111111111")]
        [InlineData("22222222222222")]
        [InlineData("XXXXXXXXXXXXXX")]
        [InlineData("1X21X21X21X21X")]
        [InlineData("112211221122XX")]
        [InlineData("X1X1X1X1X1X1X1")]
        public void RoundTrip_ConvStrToLong_ConvLongToStr_DevuelveLaMismaColumna(string columna)
        {
            long l = UtilColumnas.ConvStrToLong(columna);
            Assert.Equal(columna, UtilColumnas.ConvLongToStr(l));
        }

        [Theory]
        [InlineData("1", 4)] // 1 -> 100b
        [InlineData("X", 2)] // X -> 010b
        [InlineData("2", 1)] // 2 -> 001b
        public void ConvStrToLong_UnSigno_UsaCodificacionDeTresBits(string signo, long esperado)
        {
            Assert.Equal(esperado, UtilColumnas.ConvStrToLong(signo));
        }

        [Fact]
        public void RoundTrip_SobreColumnasRealesDeFixture()
        {
            string[] columnas = File.ReadAllLines(FixturePaths.Ruta(FixturePaths.ColsA));
            Assert.NotEmpty(columnas);

            foreach (string raw in columnas)
            {
                string col = raw.Trim();
                if (col.Length == 0) continue;

                long l = UtilColumnas.ConvStrToLong(col);
                Assert.Equal(col, UtilColumnas.ConvLongToStr(l));
            }
        }

        [Fact]
        public void Comparador_Invariante_CoincidenciasMasDiferencias_Son14_EnDatosReales()
        {
            // Para cada par (columna real de A, ganadora real), el invariante del dominio
            // coincidencias + diferencias == 14 debe cumplirse SIEMPRE.
            var cmp = new Comparador();
            string ganadora = File.ReadAllLines(FixturePaths.Ruta(FixturePaths.Ganadora))[0].Trim();
            string[] columnas = File.ReadAllLines(FixturePaths.Ruta(FixturePaths.ColsA));

            foreach (string raw in columnas)
            {
                string col = raw.Trim();
                if (col.Length == 0) continue;

                int coinc = cmp.CalculaCoincidencias(ganadora, col);
                int dif = cmp.CalculaDiferencias();
                Assert.Equal(14, coinc + dif);
                Assert.InRange(coinc, 0, 14);
            }
        }

        [Fact]
        public void Comparador_ColumnaIgualAGanadora_Da14Coincidencias_EnDatosReales()
        {
            // La ganadora del fixture coincide con la 1ª columna de cols_a -> 14.
            var cmp = new Comparador();
            string ganadora = File.ReadAllLines(FixturePaths.Ruta(FixturePaths.Ganadora))[0].Trim();
            string primeraDeA = File.ReadAllLines(FixturePaths.Ruta(FixturePaths.ColsA))[0].Trim();

            Assert.Equal(ganadora, primeraDeA); // precondición de la fixture
            Assert.Equal(14, cmp.CalculaCoincidencias(ganadora, primeraDeA));
        }
    }
}
