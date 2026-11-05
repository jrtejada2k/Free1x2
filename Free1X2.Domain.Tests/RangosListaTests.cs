using Free1X2.MotorCalculo;
using Free1X2.Utils;
using Xunit;

namespace Free1X2.Domain.Tests
{
    /// <summary>#21 (decisión del dueño, 4-nov-2026): las cantidades de las condiciones admiten rangos.</summary>
    public class RangosListaTests
    {
        [Theory]
        [InlineData("0,3,5", "0,3,5")]
        [InlineData("2-6", "2,3,4,5,6")]
        [InlineData("0,2-4,9", "0,2,3,4,9")]
        [InlineData("6-4", "4,5,6")]
        [InlineData("1-3a,7", "1a,2a,3a,7")]
        [InlineData("", "")]
        public void ExpandirLista(string entrada, string esperado)
            => Assert.Equal(esperado, RangosHelper.ExpandirLista(entrada));

        [Fact]
        public void Variantes_ConRango_IgualQueConLista()
        {
            var conRango = new FiltroNoVariantes();
            conRango.SetNoVariantes("2-6");
            var conLista = new FiltroNoVariantes();
            conLista.SetNoVariantes("2,3,4,5,6");
            Assert.Equal(conLista.GetVariantes(), conRango.GetVariantes());
        }
    }
}
