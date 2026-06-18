using Free1X2.Utils;
using Xunit;

namespace Free1X2.Domain.Tests
{
    /// <summary>
    /// Golden-master / characterization tests de RangosHelper.
    /// Bloquean el comportamiento ACTUAL del parser de rangos ("1,2-5,6")
    /// como red de seguridad antes de migrar la UI.
    /// </summary>
    public class RangosHelperTests
    {
        [Theory]
        [InlineData("3", new[] { 3 })]
        [InlineData("2-5", new[] { 2, 3, 4, 5 })]
        [InlineData("1,3,4,5", new[] { 1, 3, 4, 5 })]
        [InlineData("1,2-5,6", new[] { 1, 2, 3, 4, 5, 6 })]
        [InlineData("10-12", new[] { 10, 11, 12 })]
        public void ObtenIntArray_ParseaRangosYListas(string entrada, int[] esperado)
        {
            var helper = new RangosHelper();
            int[] actual = helper.ObtenIntArray(entrada);
            Assert.Equal(esperado, actual);
        }

        [Fact]
        public void ObtenIntArray_ValorUnico_DevuelveUnElemento()
        {
            var helper = new RangosHelper();
            Assert.Equal(new[] { 7 }, helper.ObtenIntArray("7"));
        }
    }
}
