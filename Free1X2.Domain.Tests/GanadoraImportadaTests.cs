using System.Linq;
using Free1X2.Escrutinio;
using Xunit;

namespace Free1X2.Domain.Tests
{
    /// <summary>W17 (DT-95): la ganadora de Escrutinios en las 16 casillas de «Posibles premios».</summary>
    public class GanadoraImportadaTests
    {
        [Fact]
        public void SignosYAsteriscos()
        {
            Assert.Equal(new[] { "1", "X", "2" }.Concat(Enumerable.Repeat("*", 13)),
                GanadoraImportada.SignosGanadoraImportada("1x2-"));
        }

        [Fact]
        public void Vacia_DieciseisAsteriscos()
        {
            Assert.Equal(Enumerable.Repeat("*", 16), GanadoraImportada.SignosGanadoraImportada(""));
            Assert.Equal(Enumerable.Repeat("*", 16), GanadoraImportada.SignosGanadoraImportada(null));
        }
    }
}
