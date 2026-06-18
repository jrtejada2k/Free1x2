using Free1X2.Utils;
using Xunit;

namespace Free1X2.Domain.Tests
{
    /// <summary>
    /// Characterization tests de <see cref="Comparador2"/>: variante basada en una
    /// tabla precalculada (2187 x 2187) de coincidencias entre dos mitades de 7
    /// signos. Estos tests bloquean el comportamiento ACTUAL.
    ///
    /// Convenciones caracterizadas (importantes y poco obvias):
    ///  - n1s/n2s usan 'X' para el dígito base-3 con resto 0 (NO '0'). Por tanto
    ///    el número 0 se representa como "XXXXXXX" (7) y "XXXXXXXXXXXXXX" (14).
    ///  - s1n("col14") = nx1*2187 + nx2, con nx1 = mitad izquierda (7 signos) y
    ///    nx2 = mitad derecha (7 signos), interpretando 1->1, 2->2, X/otro->0.
    ///  - neq(...) requiere haber fijado antes la columna de referencia con e1c(...).
    ///    Devuelve coincidencias en los primeros 7 + coincidencias en los últimos 7.
    /// </summary>
    public class Comparador2Tests
    {
        // --- n1s / s1n : round-trip de columna de 14 signos <-> entero ---

        [Theory]
        [InlineData("11111111111111")]
        [InlineData("22222222222222")]
        [InlineData("XXXXXXXXXXXXXX")]
        [InlineData("1X21X21X21X21X")]
        [InlineData("112211221122XX")]
        public void RoundTrip_s1n_n1s_DevuelveLaMismaColumna(string columna)
        {
            var c = new Comparador2();
            int n = c.s1n(columna);
            Assert.Equal(columna, c.n1s(n));
        }

        [Fact]
        public void s1n_ColumnaTodoX_EsCero()
        {
            var c = new Comparador2();
            Assert.Equal(0, c.s1n("XXXXXXXXXXXXXX"));
        }

        [Fact]
        public void s1n_ColumnaTodoUnos_EsValorConocido()
        {
            var c = new Comparador2();
            // Cada mitad de 7 unos = sum(3^k, k=0..6) = 1093.
            // s1n = nx1*2187 + nx2 = 1093*2187 + 1093 = 2391484.
            Assert.Equal(2391484, c.s1n("11111111111111"));
        }

        [Fact]
        public void n1s_Cero_EsTodoX()
        {
            var c = new Comparador2();
            Assert.Equal("XXXXXXXXXXXXXX", c.n1s(0));
        }

        // --- neq : coincidencias entre columna de referencia y otra ---

        [Fact]
        public void neq_ColumnasIdenticas_Da14()
        {
            var c = new Comparador2();
            int colId = c.s1n("1X21X21X21X21X");
            c.e1c(colId);                       // fija la columna de referencia
            Assert.Equal(14, c.neq(colId));     // identica -> 14 coincidencias
        }

        [Fact]
        public void neq_ColumnaTotalmenteOpuesta_Da0()
        {
            var c = new Comparador2();
            // referencia todo 1, comparada todo 2 -> 0 coincidencias en los 14.
            c.e1c(c.s1n("11111111111111"));
            Assert.Equal(0, c.neq(c.s1n("22222222222222")));
        }

        [Fact]
        public void neq_UnaDiferencia_Da13()
        {
            var c = new Comparador2();
            c.e1c(c.s1n("11111111111111"));
            Assert.Equal(13, c.neq(c.s1n("1111111111111X")));
        }

        [Fact]
        public void neq_SobrecargaDosMitades_IgualQueColnum()
        {
            var c = new Comparador2();
            int nx1 = 0, nx2 = 0;
            c.s2n("1X21X21X21X21X", ref nx1, ref nx2);

            // Fijar referencia por mitades y comparar contra si misma por mitades.
            c.e1c(nx1, nx2);
            int porMitades = c.neq(nx1, nx2);

            // Y el equivalente por colnum entero.
            c.e1c(c.s1n("1X21X21X21X21X"));
            int porColnum = c.neq(c.s1n("1X21X21X21X21X"));

            Assert.Equal(porColnum, porMitades);
            Assert.Equal(14, porMitades);
        }

        // --- s2n / n2s : mitades de 7 signos ---

        [Fact]
        public void s2n_PartePorMitadesDe7()
        {
            var c = new Comparador2();
            int nx1 = 0, nx2 = 0;
            c.s2n("11111112222222", ref nx1, ref nx2);
            // mitad izquierda = "1111111" = 1093 ; mitad derecha = "2222222" = 2186.
            Assert.Equal(1093, nx1);
            Assert.Equal(2186, nx2);
        }

        [Fact]
        public void n2s_ReconstruyeColumnaDe14DesdeDosMitades()
        {
            var c = new Comparador2();
            int nx1 = 0, nx2 = 0;
            c.s2n("1122XX1122XX11", ref nx1, ref nx2);
            Assert.Equal("1122XX1122XX11", c.n2s(nx1, nx2));
        }
    }
}
