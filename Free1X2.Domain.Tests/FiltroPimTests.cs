using System;
using Free1X2.Analisis;
using Xunit;

namespace Free1X2.Domain.Tests
{
    /// <summary>
    /// Filtro Pim con el algoritmo del original (<c>Free1X2/UI/FiltroPim.cs</c>): 7 rankings de 6 casillas y recorrido,
    /// sin suma. Caso de <c>docs/PRUEBAS_WINDOWS.md</c> (F5), calculado a mano.
    /// </summary>
    public class FiltroPimTests
    {
        private static double[,] Valores(double v1, double vx, double v2)
        {
            var v = new double[14, 3];
            for (int p = 0; p < 14; p++) { v[p, 0] = v1; v[p, 1] = vx; v[p, 2] = v2; }
            return v;
        }

        // Rangos por defecto de la pantalla: R1 3-6, R2–R5 2-6, R6 0-1, R7 0-1, recorrido 0-14.
        private static readonly int[,] RangosPorDefecto =
            { { 3, 6 }, { 2, 6 }, { 2, 6 }, { 2, 6 }, { 2, 6 }, { 0, 1 }, { 0, 1 }, { 0, 14 } };

        private static string Ranking(int[,] rk, int r)
        {
            var s = new string[14];
            for (int p = 0; p < 14; p++) s[p] = FiltroPim.Signos(rk[p, r]);
            return string.Join(" ", s);
        }

        [Fact]
        public void Rankings_60_25_15()
        {
            int[,] rk = FiltroPim.CalculaRankings(Valores(60, 25, 15));
            Assert.Equal("1 1 1 1 1 1 - - - - - - - -", Ranking(rk, 0));
            Assert.Equal("- - - - - - 1 1 1 1 1 1 - -", Ranking(rk, 1));
            Assert.Equal("X X X X - - - - - - - - 1 1", Ranking(rk, 2));
            Assert.Equal("- - - - X X X X X X - - - -", Ranking(rk, 3));
            Assert.Equal("2 2 - - - - - - - - X X X X", Ranking(rk, 4));
            Assert.Equal("- - 2 2 2 2 2 2 - - - - - -", Ranking(rk, 5));
            Assert.Equal("- - - - - - - - 2 2 2 2 2 2", Ranking(rk, 6));
        }

        [Fact]
        public void Aciertos_Recorrido_Y_Admision()
        {
            int[,] rk = FiltroPim.CalculaRankings(Valores(60, 25, 15));

            int[] a = FiltroPim.Aciertos(rk, "111XXX11X1XX12");
            Assert.Equal(new[] { 3, 3, 2, 3, 2, 0, 1 }, a);
            Assert.Equal(3, FiltroPim.Recorrido(a));
            Assert.True(FiltroPim.Admite(a, RangosPorDefecto));
            Assert.Equal(a, FiltroPim.Aciertos(rk, "111xxx11x1xx12"));

            // Catorce unos: R4 (X de P5 a P10) no tiene ningún acierto y su mínimo es 2.
            int[] unos = FiltroPim.Aciertos(rk, "11111111111111");
            Assert.Equal(new[] { 6, 6, 2, 0, 0, 0, 0 }, unos);
            Assert.False(FiltroPim.Admite(unos, RangosPorDefecto));
        }

        [Fact]
        public void Recorrido_Fuera_De_Rango_No_Admite()
        {
            int[,] rk = FiltroPim.CalculaRankings(Valores(60, 25, 15));
            int[] a = FiltroPim.Aciertos(rk, "111XXX11X1XX12");
            var rangos = (int[,])RangosPorDefecto.Clone();
            rangos[7, 0] = 4; // recorrido 4-14: el 3 queda fuera
            Assert.False(FiltroPim.Admite(a, rangos));
        }

        [Fact]
        public void Columna_Corta_Da_Error()
        {
            int[,] rk = FiltroPim.CalculaRankings(Valores(60, 25, 15));
            Assert.Throws<ArgumentException>(() => FiltroPim.Aciertos(rk, "1111"));
        }
    }
}
