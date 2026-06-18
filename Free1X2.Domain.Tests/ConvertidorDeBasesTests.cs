using System.IO;
using Free1X2.Utils;
using Xunit;

namespace Free1X2.Domain.Tests
{
    /// <summary>
    /// Golden-master de <see cref="ConvertidorDeBases"/>: la columna 1/X/2 de 14
    /// partidos se interpreta como un número en base 3 (1->0, X->1, 2->2 por dígito).
    /// El round-trip número -> columna -> número debe ser la identidad, y la columna
    /// canónica de un número debe re-convertir al mismo número.
    ///
    /// NOTA caracterizada: ConvNumAColumna SIEMPRE rellena con '1' (el dígito 0) a la
    /// derecha hasta NumeroPartidos, por lo que solo es una identidad string<->string
    /// para columnas ya "canónicas" (las que produce el propio conversor). El round-trip
    /// fiable y verificado aquí es número -> columna -> número.
    /// </summary>
    public class ConvertidorDeBasesTests
    {
        // 14 partidos fijos: independiente de la config global, así el test es determinista.
        private static ConvertidorDeBases Conv() => new ConvertidorDeBases(14);

        [Theory]
        [InlineData("11111111111111", 0)]                 // todo '1' = todos los dígitos 0 = número 0
        [InlineData("X1111111111111", 1)]                 // X en P1 = 1*3^0
        [InlineData("21111111111111", 2)]                 // 2 en P1 = 2*3^0
        [InlineData("1X111111111111", 3)]                 // X en P2 = 1*3^1
        public void ConvColumnaANumero_CasosConocidos(string columna, int esperado)
        {
            Assert.Equal(esperado, Conv().ConvColumnaANumero(columna));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(80)]
        [InlineData(1234)]
        [InlineData(100000)]
        [InlineData(2391484)]
        [InlineData(4782968)] // 3^14 - 1, el número máximo de 14 partidos
        public void RoundTrip_NumeroAColumnaYVuelta_EsLaIdentidad(int numero)
        {
            var c = Conv();
            string columna = c.ConvNumAColumna(numero);
            Assert.Equal(14, columna.Length);
            Assert.Equal(numero, c.ConvColumnaANumero(columna));
        }

        [Fact]
        public void ConvNumAColumna_NumeroMaximo_EsTodoDoses()
        {
            // 3^14 - 1 = todos los dígitos a 2 -> columna de 14 doses.
            Assert.Equal("22222222222222", Conv().ConvNumAColumna(4782968));
        }

        [Fact]
        public void RoundTrip_SobreColumnasRealesDeFixture()
        {
            // Cada columna real del fichero, convertida a número y de vuelta a su forma
            // canónica, debe re-convertir exactamente al mismo número.
            var c = Conv();
            string[] columnas = File.ReadAllLines(FixturePaths.Ruta(FixturePaths.ColsA));
            Assert.NotEmpty(columnas);

            foreach (string raw in columnas)
            {
                string col = raw.Trim();
                if (col.Length == 0) continue;

                int num = c.ConvColumnaANumero(col);
                string canonica = c.ConvNumAColumna(num);
                Assert.Equal(num, c.ConvColumnaANumero(canonica));
                // Y el número está dentro del universo de 14 partidos (3^14).
                Assert.InRange(num, 0, 4782968);
            }
        }
    }
}
