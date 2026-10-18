using System;
using System.Globalization;
using System.Linq;
using Free1X2.Utils;
using Xunit;

namespace Free1X2.Domain.Tests
{
    /// <summary>
    /// W16 (DT-93): precio y recaudación de Configuración en lugar de 0,50 € y 14.000.000 € fijos. Mismos casos que la
    /// app Mac (<c>PrecioConfigurableTests</c> en <c>TramificarTests.swift</c>).
    /// </summary>
    public class PrecioConfigurableTests
    {
        private static readonly CultureInfo Es = new("es-ES");
        private static readonly double[] Premios = { 1165207.35, 29877.11, 1753.51, 165.38, 22.79 };

        [Fact]
        public void GuardarJornadaLAE_A075_SeLeeIgualConEstimacionA075()
        {
            string linea = LineaJornadaLAE.Componer("1X21X1121X1X21", "2026/2027", "7", "9000000",
                new[] { "1165207,35", "29877,11", "1753,51", "165,38", "22,79" }, 0.75, 14000000, Es);
            string[] c = linea.Split('\t');
            Assert.Equal(9, c.Length);
            Assert.Equal(new[] { "1X21X1121X1X21", "2026/2027", "7" }, c.Take(3));

            // Estimación de premios lee cada importe × su precio (aquí 0,75).
            double Leer(string v) => double.Parse(v, Es) * 0.75;
            Assert.True(Math.Abs(Leer(c[3]) - 9000000) < 1e-6);
            for (int i = 0; i < 5; i++)
                Assert.True(Math.Abs(Leer(c[4 + i]) - Premios[i]) < 1e-6, $"premio {14 - i}");
        }

        [Fact]
        public void GuardarJornadaLAE_RecaudacionVacia_UsaLaDeConfiguracion()
        {
            string linea = LineaJornadaLAE.Componer("1", "2026/2027", "7", "", new string[5], 0.75, 9000000, Es);
            Assert.Equal("12000000", linea.Split('\t')[3]);
        }

        [Fact]
        public void GuardarJornadaLAE_A050_ComoElOriginal()
        {
            // Con 0,50 y la recaudación vacía, lo mismo que hacía el original (14.000.000 ÷ 0,5).
            string linea = LineaJornadaLAE.Componer("1", "2026/2027", "7", "", new[] { "10", "", "", "", "" }, 0.5, 14000000, Es);
            Assert.Equal(new[] { "28000000", "20", "0", "0", "0", "0" }, linea.Split('\t').Skip(3));
        }

        [Fact]
        public void BalanceDelTramo_ConElPrecio()
        {
            var tr = new Tramo(new double[] { 1000, 0, 0, 0, 0 }) { NumColumnasTramo = 100 };
            tr.PonerAciertos(new[] { 1, 0, 0, 0, 0 });
            tr.CalculaTotalImportePremios(0.75);
            Assert.Equal(925, tr.Balance, 6);
            tr.CalculaTotalImportePremios(0.5);
            Assert.Equal(950, tr.Balance, 6);
            tr.CalculaTotalImportePremios();
            Assert.Equal(950, tr.Balance, 6);
        }
    }
}
