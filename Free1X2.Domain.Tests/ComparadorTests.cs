using Free1X2.Utils;
using Xunit;

namespace Free1X2.Domain.Tests
{
    /// <summary>
    /// Golden-master de Comparador: coincidencias/diferencias entre dos columnas
    /// de 14 signos (1/X/2). Es lógica núcleo del escrutinio y la reducción.
    /// </summary>
    public class ComparadorTests
    {
        [Fact]
        public void ColumnasIdenticas_14Coincidencias()
        {
            var c = new Comparador();
            Assert.Equal(14, c.CalculaCoincidencias("11111111111111", "11111111111111"));
        }

        [Fact]
        public void UnaDiferencia_13Coincidencias()
        {
            var c = new Comparador();
            // difieren solo en el último signo
            Assert.Equal(13, c.CalculaCoincidencias("11111111111111", "1111111111111X"));
        }

        [Fact]
        public void TresDiferencias_11Coincidencias()
        {
            var c = new Comparador();
            // difieren en las 3 primeras posiciones (1->X, 1->X, 1->2)
            Assert.Equal(11, c.CalculaCoincidencias("11111111111111", "XX211111111111"));
        }

        [Fact]
        public void Coincidencias_MasDiferencias_SumanCatorce()
        {
            var c = new Comparador { col1 = "1X21X21X21X21X", col2 = "112211221122XX" };
            int coinc = c.CalculaCoincidencias();
            int dif = c.CalculaDiferencias();
            // Invariante del dominio: coincidencias + diferencias = 14
            Assert.Equal(14, coinc + dif);
        }
    }
}
