using Free1X2.Utils;
using Xunit;

namespace Free1X2.Domain.Tests
{
    /// <summary>
    /// Golden-master de la API de bytes de <see cref="Comparador"/>:
    /// conversión columna(string) -> 3 bytes y vuelta, y las sobrecargas de
    /// CalculaCoincidencias / CalculaDiferencias que operan sobre byte[].
    ///
    /// NOTA de comportamiento (caracterizado, no es un bug a corregir aquí):
    /// ConvColumnaA3Bytes empaqueta 14 signos en 3 bytes de 5 "tritos" cada uno
    /// (15 posiciones); para rellenar la posición 15 el método añade un "1"
    /// fijo al final de la columna (columna1 += "1"). Por eso el round-trip
    /// string -> bytes -> string SIEMPRE reaparece con un "1" en la posición 15
    /// pero Conv3BytesAColumna recorta a 14 con Substring(0,4) en el 3er byte,
    /// devolviendo exactamente los 14 signos originales.
    /// </summary>
    public class ComparadorBytesTests
    {
        [Theory]
        [InlineData("11111111111111")]
        [InlineData("XXXXXXXXXXXXXX")]
        [InlineData("22222222222222")]
        [InlineData("1X21X21X21X21X")]
        [InlineData("112211221122XX")]
        [InlineData("X1X1X1X1X1X1X1")]
        public void RoundTrip_ColumnaABytesYVuelta_DevuelveLaMismaColumna(string columna)
        {
            var c = new Comparador();
            byte b1 = 0, b2 = 0, b3 = 0;
            c.ConvColumnaA3Bytes(columna, ref b1, ref b2, ref b3);

            string vuelta = c.Conv3BytesAColumna(b1, b2, b3);
            Assert.Equal(columna, vuelta);
        }

        [Fact]
        public void RoundTrip_PropiedadCol1_DevuelveLaMismaColumna()
        {
            var c = new Comparador { col1 = "1X21X21X21X21X" };
            Assert.Equal("1X21X21X21X21X", c.col1);
        }

        [Fact]
        public void RoundTrip_PropiedadCol2_DevuelveLaMismaColumna()
        {
            var c = new Comparador { col2 = "221X1X221X1X22" };
            Assert.Equal("221X1X221X1X22", c.col2);
        }

        [Fact]
        public void Conv3BytesAColumna_SobrecargaArray_IgualQueSobrecargaTresBytes()
        {
            var c = new Comparador();
            byte b1 = 0, b2 = 0, b3 = 0;
            c.ConvColumnaA3Bytes("12X12X12X12X12", ref b1, ref b2, ref b3);

            string viaTresBytes = c.Conv3BytesAColumna(b1, b2, b3);
            string viaArray = c.Conv3BytesAColumna(new[] { b1, b2, b3 });
            Assert.Equal(viaTresBytes, viaArray);
        }

        [Fact]
        public void CalculaCoincidencias_SobrecargaArray_OperaSobreCol1()
        {
            // col1 fija; comparamos un byte[] que representa OTRA columna contra col1.
            var c = new Comparador { col1 = "11111111111111" };
            byte b1 = 0, b2 = 0, b3 = 0;
            // columna que difiere de col1 en las 3 primeras posiciones
            c.ConvColumnaA3Bytes("XX211111111111", ref b1, ref b2, ref b3);

            // CalculaCoincidencias(byte[]) compara b contra byteCol1.
            Assert.Equal(11, c.CalculaCoincidencias(new[] { b1, b2, b3 }));
        }

        [Fact]
        public void CalculaDiferencias_SobrecargaArray_EsComplementoDeCoincidencias()
        {
            var c = new Comparador { col1 = "1X21X21X21X21X" };
            byte b1 = 0, b2 = 0, b3 = 0;
            c.ConvColumnaA3Bytes("112211221122XX", ref b1, ref b2, ref b3);

            int coinc = c.CalculaCoincidencias(new[] { b1, b2, b3 });
            int dif = c.CalculaDiferencias(new[] { b1, b2, b3 });
            Assert.Equal(14, coinc + dif);
        }

        [Fact]
        public void CalculaCoincidenciasConCol2_OperaSobreCol2()
        {
            var c = new Comparador { col2 = "11111111111111" };
            byte b1 = 0, b2 = 0, b3 = 0;
            c.ConvColumnaA3Bytes("XX211111111111", ref b1, ref b2, ref b3);

            // CalculaCoincidenciasConCol2 compara los bytes dados contra byteCol2.
            Assert.Equal(11, c.CalculaCoincidenciasConCol2(b1, b2, b3));
        }

        [Theory]
        [InlineData("11111111111111", "11111111111111", 14)]
        [InlineData("11111111111111", "1111111111111X", 13)]
        [InlineData("11111111111111", "XX211111111111", 11)]
        [InlineData("11111111111111", "XXXXXXXXXXXXXX", 0)]
        [InlineData("11111111111111", "22222222222222", 0)]
        public void CalculaCoincidencias_StringString_CasosConocidos(string a, string b, int esperado)
        {
            var c = new Comparador();
            Assert.Equal((byte)esperado, c.CalculaCoincidencias(a, b));
        }

        [Theory]
        // Para CUALQUIER par de columnas de 14 signos: coincidencias + diferencias = 14.
        [InlineData("1X21X21X21X21X", "112211221122XX")]
        [InlineData("XXXXXXXXXXXXXX", "11111111111111")]
        [InlineData("12121212121212", "21212121212121")]
        [InlineData("1122XX1122XX11", "XX1122XX1122XX")]
        public void Invariante_CoincidenciasMasDiferencias_SonCatorce(string a, string b)
        {
            var c = new Comparador();
            int coinc = c.CalculaCoincidencias(a, b);
            // CalculaCoincidencias(a,b) ya fijó byteCol1=a y byteCol2=b internamente.
            int dif = c.CalculaDiferencias();
            Assert.Equal(14, coinc + dif);
        }
    }
}
