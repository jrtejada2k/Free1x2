using System;
using System.Linq;
using Free1X2.Analisis;
using Xunit;

namespace Free1X2.Domain.Tests
{
    /// <summary>
    /// W19 (DT-95): orden y «Filtro de selección de filas» del Banco de pruebas. Mismo caso que la app Mac
    /// (<c>BancoPruebasTests.ordenYFiltro</c>): autoescrutinio de 11111111111111, X1111111111111 y 22222222222222 con el
    /// 14 y el 13 marcados. Las dos primeras se dan 14 y 13 entre sí; la última, solo su propio 14.
    /// </summary>
    public class BancoPruebasTablaTests
    {
        private static readonly bool[] Acumula14y13 = { true, true, false, false, false };

        private static FilaBancoColumna[] Filas() => new[]
        {
            new FilaBancoColumna(0, "11111111111111", new[] { 1, 1, 0, 0, 0 }, new[] { 900.0, 40.4, 0, 0, 0 }, Acumula14y13, 3, 0.5),
            new FilaBancoColumna(1, "X1111111111111", new[] { 1, 1, 0, 0, 0 }, new[] { 800.0, 30.6, 0, 0, 0 }, Acumula14y13, 3, 0.5),
            new FilaBancoColumna(2, "22222222222222", new[] { 1, 0, 0, 0, 0 }, new[] { 1500.0, 0, 0, 0, 0 }, Acumula14y13, 3, 0.5),
        };

        [Fact]
        public void ValoresDeLaFila()
        {
            var f = Filas();
            Assert.Equal(new[] { 2, 2, 1 }, f.Select(x => x.VecesAcumulado));
            Assert.True(f[0].Premios[1] > 0);
            Assert.Equal(0, f[2].Premios[1]);
            Assert.Equal(Math.Round(f[0].Premios[0] + f[0].Premios[1], 1), f[0].PremioAcumulado);
            Assert.Equal(Math.Round(f[0].Premios[1]), f[0].PremioMedio(1));
            Assert.Equal(Math.Round(940.4 * 100 / 3 / 0.5, 0), f[0].PorcentajeRecuperacion);
        }

        [Fact]
        public void Orden()
        {
            var f = Filas();
            // «Nº13» de mayor a menor deja la tercera al final; la segunda pulsación la pone al principio.
            Assert.True(BancoPruebasTabla.DescendentePorDefecto(2));
            Assert.Equal(2, BancoPruebasTabla.Ordenar(f, x => x, 2, true).Last().Indice);
            Assert.Equal(2, BancoPruebasTabla.Ordenar(f, x => x, 2, false).First().Indice);
            // «Nºcol.» de menor a mayor.
            Assert.False(BancoPruebasTabla.DescendentePorDefecto(0));
            var porCol = BancoPruebasTabla.Ordenar(BancoPruebasTabla.Ordenar(f, x => x, 2, false), x => x, 0, false);
            Assert.Equal(new[] { 0, 1, 2 }, porCol.Select(x => x.Indice));
        }

        [Fact]
        public void LimitesYSeleccion()
        {
            var f = Filas();
            var l = BancoPruebasTabla.Limites(f);
            Assert.Equal(18, l.Length);
            Assert.Equal((0d, 1d), l[1]);
            Assert.Equal((1d, 2d), l[5]);

            var nada = new bool[3];
            // «Nº de veces 13 aciertos» entre 1 y 1 ⇒ las dos primeras.
            var s = BancoPruebasTabla.Seleccionar(f, nada, new[] { (1, 1d, 1d) });
            Assert.Equal(new[] { true, true, false }, s);
            // El primer concepto que falla desmarca: «con premio» 0–9 y «Nº de veces 14» 2–9 ⇒ ninguna.
            s = BancoPruebasTabla.Seleccionar(f, s, new[] { (5, 0d, 9d), (0, 2d, 9d) });
            Assert.Equal(0, s.Count(x => x));
            // Sin conceptos no cambia nada.
            s[2] = true;
            s = BancoPruebasTabla.Seleccionar(f, s, Array.Empty<(int, double, double)>());
            Assert.Equal(1, s.Count(x => x));
        }
    }
}
