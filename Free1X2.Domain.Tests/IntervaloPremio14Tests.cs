using System;
using Free1X2.Utils;
using Xunit;

namespace Free1X2.Domain.Tests
{
    /// <summary>
    /// W18 (DT-95): «Obtener intervalo: premio de 14 entre … y …» de Ordenar por probabilidad. Mismos datos que la app
    /// Mac (<c>OrdenarPorProbabilidadTests.intervaloPremio14</c>): valoraciones 50 / 30 / 20 en los 14 partidos,
    /// recaudación 15.000.000, precio 0,5 y %14 15, pestaña «Por productos» (probabilidad = producto).
    /// </summary>
    public class IntervaloPremio14Tests
    {
        private static float Premio(float prob) => IntervaloPremio14.Premio(0.5, 15, 15000000, prob);

        // 11111111111111: 0,5¹⁴. 2222222222222X: 0,2¹³ · 0,3.
        private static readonly float Unos = (float)Math.Pow(0.5, 14);
        private static readonly float DosesX = (float)(Math.Pow(0.2, 13) * 0.3);

        [Fact]
        public void PremioYTope()
        {
            Assert.Equal(1228.8f, Premio(Unos), 1);       // 0,075 ÷ 0,5¹⁴
            Assert.Equal(2250000f, Premio(DosesX));       // tope: 15.000.000 × 15 %
        }

        [Fact]
        public void Intervalos()
        {
            // 1.000–2.000 ⇒ solo la de unos; 2.000.000–3.000.000 ⇒ solo 2…2X; 0–1.000 ⇒ ninguna.
            Assert.True(IntervaloPremio14.Admite(Premio(Unos), 1000, 2000));
            Assert.False(IntervaloPremio14.Admite(Premio(DosesX), 1000, 2000));
            Assert.False(IntervaloPremio14.Admite(Premio(Unos), 2000000, 3000000));
            Assert.True(IntervaloPremio14.Admite(Premio(DosesX), 2000000, 3000000));
            Assert.False(IntervaloPremio14.Admite(Premio(Unos), 0, 1000));
            Assert.False(IntervaloPremio14.Admite(Premio(DosesX), 0, 1000));
        }
    }
}
