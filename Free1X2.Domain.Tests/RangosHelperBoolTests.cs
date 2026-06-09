using Free1X2.Utils;
using Xunit;

namespace Free1X2.Domain.Tests
{
    /// <summary>
    /// Golden-master / characterization de <see cref="RangosHelper.ObtenBoolArray"/>:
    /// parser de rangos ("1,3-5,7") a bool[] indexado por posición.
    ///
    /// Comportamiento caracterizado (no es un bug a corregir aquí):
    ///  - El array resultante tiene longitud (maximo + 2). El "+2" deja un hueco
    ///    final siempre false e implica que el indice 0 existe (y queda false salvo
    ///    que el rango incluya el 0 explicitamente).
    ///  - Las posiciones listadas/rango quedan en true; el resto en false.
    /// </summary>
    public class RangosHelperBoolTests
    {
        [Fact]
        public void ObtenBoolArray_ValorUnico_LongitudYPosicion()
        {
            var helper = new RangosHelper();
            bool[] r = helper.ObtenBoolArray("3");

            // max = 3 -> longitud = 3 + 2 = 5 (indices 0..4).
            Assert.Equal(5, r.Length);
            Assert.True(r[3]);
            // todas las demas false
            Assert.False(r[0]);
            Assert.False(r[1]);
            Assert.False(r[2]);
            Assert.False(r[4]);
        }

        [Fact]
        public void ObtenBoolArray_Rango_MarcaTodoElIntervalo()
        {
            var helper = new RangosHelper();
            bool[] r = helper.ObtenBoolArray("2-5");

            // max = 5 -> longitud = 7.
            Assert.Equal(7, r.Length);
            Assert.False(r[0]);
            Assert.False(r[1]);
            Assert.True(r[2]);
            Assert.True(r[3]);
            Assert.True(r[4]);
            Assert.True(r[5]);
            Assert.False(r[6]);
        }

        [Fact]
        public void ObtenBoolArray_ListaYRangoCombinados()
        {
            var helper = new RangosHelper();
            bool[] r = helper.ObtenBoolArray("1,3-5,7");

            // max = 7 -> longitud = 9 (indices 0..8).
            Assert.Equal(9, r.Length);

            bool[] esperado = { false, true, false, true, true, true, false, true, false };
            Assert.Equal(esperado, r);
        }

        [Fact]
        public void ObtenBoolArray_ListaSimple()
        {
            var helper = new RangosHelper();
            bool[] r = helper.ObtenBoolArray("1,3,4,5");

            // max = 5 -> longitud = 7.
            bool[] esperado = { false, true, false, true, true, true, false };
            Assert.Equal(esperado, r);
        }

        [Fact]
        public void ObtenBoolArray_PosicionesMarcadasCoincidenConObtenIntArray()
        {
            // Invariante cruzada: los indices true de ObtenBoolArray son exactamente
            // los valores devueltos por ObtenIntArray para la misma entrada.
            var helper = new RangosHelper();
            const string entrada = "1,2-5,6";

            int[] enteros = helper.ObtenIntArray(entrada);
            bool[] bools = helper.ObtenBoolArray(entrada);

            foreach (int idx in enteros)
            {
                Assert.True(bools[idx], $"se esperaba bools[{idx}] == true");
            }

            // y el numero de trues coincide con el numero de enteros (sin duplicados).
            int trues = 0;
            foreach (bool b in bools)
            {
                if (b) trues++;
            }
            Assert.Equal(enteros.Length, trues);
        }
    }
}
