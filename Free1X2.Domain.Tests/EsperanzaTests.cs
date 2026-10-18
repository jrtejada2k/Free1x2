using System;
using System.Linq;
using Free1X2.Analisis;
using Free1X2.Utils;
using Xunit;

namespace Free1X2.Domain.Tests
{
    /// <summary>
    /// W14 (DT-91): EM exacta por categoría, contra el ejemplo del Excel «EM Categorias.xlsm» del dueño (mismos datos y
    /// esperados que <c>EsperanzaTests.swift</c> de la app Mac). W15 (DT-92): «Pegar desde Excel».
    /// </summary>
    public class EsperanzaTests
    {
        private static readonly double[,] Apostados =
        {
            { 65, 26, 9 }, { 8, 16, 76 }, { 15, 25, 60 }, { 77, 16, 7 }, { 45, 37, 18 }, { 21, 32, 47 }, { 91, 6, 3 },
            { 59, 27, 14 }, { 29, 39, 32 }, { 68, 22, 10 }, { 31, 35, 34 }, { 43, 35, 22 }, { 76, 17, 7 }, { 13, 24, 63 },
        };

        private static readonly double[,] Reales =
        {
            { 50.483, 29.487, 20.03 }, { 17.834, 25.144, 57.022 }, { 19.196, 27.523, 53.282 }, { 61.776, 21.575, 16.649 },
            { 45.775, 30.493, 23.732 }, { 22.847, 27.438, 49.714 }, { 76.773, 14.886, 8.341 }, { 48.195, 25.625, 26.18 },
            { 37.232, 33.017, 29.751 }, { 50.901, 33.516, 15.583 }, { 35.146, 33.792, 31.062 }, { 45.181, 30.249, 24.57 },
            { 56.921, 29.003, 14.077 }, { 22.433, 35.253, 42.314 },
        };

        private const string Columna = "1121121221112X";
        private const double Recaudacion = 3153494.25;

        private static ResultadoEsperanza Em(double precio, bool masUno = true) =>
            EsperanzaMatematica.Calcular(Columna, Apostados, Reales,
                new ParametrosEsperanza(Recaudacion, precio) { ContarmeComoAcertante = masUno });

        private static void Cerca(double[] esperado, double[] real)
        {
            Assert.Equal(esperado.Length, real.Length);
            for (int i = 0; i < esperado.Length; i++)
                Assert.True(Math.Abs(esperado[i] - real[i]) < 1e-9 * Math.Max(1, Math.Abs(esperado[i])),
                    $"[{i}] esperado {esperado[i]:R}, real {real[i]:R}");
        }

        [Fact]
        public void Distribucion()
        {
            var d = EsperanzaMatematica.Distribucion(Enumerable.Repeat(0.6, 14).ToArray());
            Assert.True(Math.Abs(d[14] - Math.Pow(0.6, 14)) < 1e-15);
            Assert.True(Math.Abs(d[13] - 14 * Math.Pow(0.6, 13) * 0.4) < 1e-15);
            Assert.True(Math.Abs(d.Sum() - 1) < 1e-12);
        }

        [Fact]
        public void EjemploDelExcel()
        {
            var uno = Em(1);
            Cerca(new[] { 0.339387083746, 0.259952860574, 0.199802964982, 0.145122171329, 0.129636490232 },
                uno.Categorias.Select(c => c.Esperanza).ToArray());
            Assert.True(Math.Abs(uno.Total - 1.073901570862) < 1e-9);
            Assert.True(Math.Abs(uno.Rendimiento - 1.073901570862) < 1e-9);

            var tresCuartos = Em(0.75);
            Assert.True(Math.Abs(tresCuartos.Total - 0.894180459434) < 1e-9);
            Assert.True(Math.Abs(tresCuartos.Apuestas - 4204659) < 1e-6);
            Assert.True(Math.Abs(tresCuartos.Rendimiento - 0.894180459434 / 0.75) < 1e-9);

            Assert.True(Math.Abs(Em(1, masUno: false).Total - 1.423042097518) < 1e-9);

            Cerca(new[] { 3.154448497271e-07, 1.394103042827e-05, 2.494557948604e-04, 2.385222412527e-03, 1.370617492755e-02 },
                uno.Categorias.Select(c => c.ProbabilidadApostada).ToArray());
            Cerca(new[] { 1.617487413171e-06, 4.323052551039e-05, 5.083017408476e-04, 3.496904024709e-03, 1.577808400922e-02 },
                uno.Categorias.Select(c => c.ProbabilidadReal).ToArray());
            Assert.Equal(new[] { 14, 13, 12, 11, 10 }, uno.Categorias.Select(c => c.Aciertos));
        }

        [Fact]
        public void RetencionYMinimos()
        {
            var p = new ParametrosEsperanza(Recaudacion, 1);
            var con = EsperanzaMatematica.Calcular(Columna, Apostados, Reales, p);
            p.Retencion = 0;
            var sin = EsperanzaMatematica.Calcular(Columna, Apostados, Reales, p);
            Assert.True(sin.Categorias[0].Esperanza > con.Categorias[0].Esperanza);
            p.Minimo10 = 1000000;
            var sin10 = EsperanzaMatematica.Calcular(Columna, Apostados, Reales, p);
            Assert.Equal(0, sin10.Categorias[4].Esperanza);
            Assert.Equal(sin.Categorias[0].Esperanza, sin10.Categorias[0].Esperanza);
        }

        [Fact]
        public void Errores()
        {
            var p = new ParametrosEsperanza(Recaudacion, 1);
            Assert.Throws<ArgumentException>(() => EsperanzaMatematica.Calcular("112112", Apostados, Reales, p));
            var ceros = (double[,])Reales.Clone();
            ceros[3, 0] = 0; ceros[3, 1] = 0; ceros[3, 2] = 0;
            var e = Assert.Throws<ArgumentException>(() => EsperanzaMatematica.Calcular(Columna, Apostados, ceros, p));
            Assert.Equal("Faltan los porcentajes reales del partido 4.", e.Message);
            Assert.Throws<ArgumentException>(() =>
                EsperanzaMatematica.Calcular(Columna, Apostados, Reales, new ParametrosEsperanza(1, 0)));
        }

        [Fact]
        public void PegarDesdeExcel_FilasDeExcel()
        {
            string texto = "Part.\tPartido\t1\tX\t2\r\n1\tVILLARREAL-NAPOLI\t50,483\t29,487\t20,03\r\n" +
                           "7\tFEYENOORD-COMO 1907\t45\t30\t25\r\n\r\n3 ; 60% ; 25% ; 15%\n";
            var r = PorcentajesPegados.Leer(texto, 14);
            Assert.Equal(3, r.Count);
            Assert.Equal(new[] { 50.483, 29.487, 20.03 }, r[0]);
            Assert.Equal(new double[] { 45, 30, 25 }, r[1]);
            Assert.Equal(new double[] { 60, 25, 15 }, r[2]);

            var dos = PorcentajesPegados.Leer("65.5 26 8.5\n8 16 76\n15 25 60", 2);
            Assert.Equal(2, dos.Count);
            Assert.Equal(new[] { 65.5, 26, 8.5 }, dos[0]);
            Assert.Equal(new double[] { 8, 16, 76 }, dos[1]);
        }

        [Fact]
        public void PegarDesdeExcel_Errores()
        {
            Assert.Throws<FormatException>(() => PorcentajesPegados.Leer("1 X 2\nhola", 14));
            Assert.Throws<FormatException>(() => PorcentajesPegados.Leer("50\t-10\t60", 14));
        }
    }
}
