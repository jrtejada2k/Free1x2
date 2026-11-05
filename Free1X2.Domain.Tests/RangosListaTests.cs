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
        [InlineData("1-3a,7", "1A,2A,3A,7")] // D-CT2: la letra, en mayúsculas
        [InlineData("8a", "8A")]
        [InlineData("", "")]
        public void ExpandirLista(string entrada, string esperado)
            => Assert.Equal(esperado, RangosHelper.ExpandirLista(entrada));

        [Theory]
        [InlineData("0,2-3", "0,2,3")]
        [InlineData("3,1", "1,3")]
        [InlineData("7,9-11", "7,9,10,11")]
        [InlineData("0-1", "0,1")]
        [InlineData("2", "2")]
        public void ListaOrdenada(string entrada, string esperado)
            => Assert.Equal(esperado, RangosHelper.ListaOrdenada(entrada));

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
