using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Free1X2.Analisis;
using Xunit;

namespace Free1X2.Domain.Tests
{
    /// <summary>
    /// W3 (C2, DT-79): paneles «% COMBINACIÓN» / «Valoraciones JORNADA», propuesta automática y
    /// «Transposición automática» de la Rotación de signos. Mismos casos que la app Mac:
    /// <c>TransponedorAutomaticoTests.swift</c> y <c>f30</c>/<c>f33</c> de <c>FlujosTests.swift</c>.
    /// </summary>
    public class RotacionTests
    {
        // JD-01: partidos 1–6 con 1,X,2 y 7–14 con 1 (729 columnas).
        private static List<string> SeisTriples()
        {
            var columnas = new List<string>();
            const string s = "1X2";
            for (int n = 0; n < 729; n++)
            {
                var c = new char[14];
                int r = n;
                for (int p = 5; p >= 0; p--) { c[p] = s[r % 3]; r /= 3; }
                for (int p = 6; p < 14; p++) c[p] = '1';
                columnas.Add(new string(c));
            }
            return columnas;
        }

        private static double[,] Matriz(IEnumerable<double[]> filas)
        {
            var l = filas.ToList();
            var m = new double[l.Count, 3];
            for (int i = 0; i < l.Count; i++)
                for (int j = 0; j < 3; j++) m[i, j] = l[i][j];
            return m;
        }

        private static double[,] Repetir(double[] fila, int n) => Matriz(Enumerable.Repeat(fila, n));

        // Fila i con concentración estrictamente decreciente: [1 − a, a/2, a/2], a = 0,05·i.
        private static IEnumerable<double[]> Decreciente() =>
            Enumerable.Range(0, 14).Select(i => { double a = 0.05 * i; return new[] { 1 - a, a / 2, a / 2 }; });

        private static string Temporal(IEnumerable<string> lineas = null)
        {
            string ruta = Path.Combine(Path.GetTempPath(), "free1x2_rot_" + Guid.NewGuid().ToString("N") + ".txt");
            if (lineas != null) File.WriteAllLines(ruta, lineas);
            return ruta;
        }

        [Fact]
        public void Transponedor_IdentidadYPermutacion()
        {
            var dec = Matriz(Decreciente());
            Assert.Equal(Enumerable.Range(0, 14), new TransponedorAutomatico(dec, dec).Trasposicion());

            var alReves = Matriz(Decreciente().Reverse());
            Assert.Equal(Enumerable.Range(0, 14).Reverse(), new TransponedorAutomatico(dec, alReves).Trasposicion());
        }

        [Fact]
        public void Transponedor_EmpatesNoEstables()
        {
            var v = Repetir(new double[] { 20, 30, 50 }, 14);
            var f = Matriz(Enumerable.Repeat(new double[] { 34, 33, 33 }, 6)
                .Concat(Enumerable.Repeat(new double[] { 100, 0, 0 }, 8)));
            Assert.Equal(new[] { 12, 13, 8, 9, 10, 11, 0, 1, 2, 3, 4, 5, 6, 7 },
                new TransponedorAutomatico(v, f).Trasposicion());

            var uniforme = Repetir(new double[] { 1, 1, 1 }, 14);
            Assert.Equal(Enumerable.Range(0, 14), new TransponedorAutomatico(uniforme, uniforme).Trasposicion());
        }

        [Fact]
        public void Transponedor_NaNYTamano()
        {
            var uniforme = Repetir(new double[] { 1, 1, 1 }, 14);
            var v = Repetir(new double[] { 1, 1, 1 }, 14);
            v[0, 0] = 0; v[0, 1] = 0; v[0, 2] = 0;
            Assert.Equal(Enumerable.Range(0, 14), RotacionSignos.TrasposicionAutomatica(v, uniforme));
            Assert.Null(RotacionSignos.TrasposicionAutomatica(Repetir(new double[] { 1, 1, 1 }, 13), uniforme));
        }

        [Fact]
        public void PorcentajesYPropuesta_JD01()
        {
            var pct = RotacionSignos.Porcentajes(SeisTriples(), 14);
            var esperado = Matriz(Enumerable.Repeat(new double[] { 34, 33, 33 }, 6)
                .Concat(Enumerable.Repeat(new double[] { 100, 0, 0 }, 8)));
            Assert.Equal(esperado, pct);

            var jornada = Repetir(new double[] { 20, 30, 50 }, 14);
            var nuevos = RotacionSignos.PropuestaAutomatica(pct, jornada);
            for (int i = 0; i < 14; i++)
                Assert.Equal(new[] { 2, 1, 0 }, new[] { nuevos[i, 0], nuevos[i, 1], nuevos[i, 2] });

            var uno = RotacionSignos.PropuestaAutomatica(Matriz(new[] { new double[] { 40, 40, 20 } }),
                Matriz(new[] { new double[] { 10, 20, 70 } }));
            Assert.Equal(new[] { 2, 1, 0 }, new[] { uno[0, 0], uno[0, 1], uno[0, 2] });
        }

        [Fact]
        public void Porcentajes_SinColumnas_Ceros()
        {
            Assert.Equal(new double[14, 3], RotacionSignos.Porcentajes(Array.Empty<string>(), 14));
        }

        [Fact]
        public void TransponerColumnas_OrdenManual_F30()
        {
            string e = Temporal(SeisTriples());
            string s = Temporal();
            try
            {
                int[] orden = new[] { 7, 2, 3, 4, 5, 6, 1, 8, 9, 10, 11, 12, 13, 14 }.Select(x => x - 1).ToArray();
                RotacionSignos.TransponerColumnas(e, s, orden);
                var t = File.ReadAllLines(s);
                Assert.Equal(729, t.Length);
                Assert.All(t, c => Assert.Equal('1', c[0]));
                Assert.Equal(729, t.Distinct().Count());
            }
            finally { File.Delete(e); File.Delete(s); }
        }

        [Fact]
        public void TransposicionAutomatica_JD01_F33()
        {
            string e = Temporal(SeisTriples());
            string s = Temporal();
            try
            {
                var pct = RotacionSignos.Porcentajes(e, 14);
                var jornada = Repetir(new double[] { 20, 30, 50 }, 14);
                int[] orden = RotacionSignos.TrasposicionAutomatica(jornada, pct);
                Assert.Equal(new[] { 12, 13, 8, 9, 10, 11, 0, 1, 2, 3, 4, 5, 6, 7 }, orden);

                RotacionSignos.TransponerColumnas(e, s, orden);
                var t = File.ReadAllLines(s);
                Assert.Equal(729, t.Length);
                Assert.All(t, c => { Assert.StartsWith("111111", c); Assert.EndsWith("11", c); });
                Assert.Equal(729, t.Select(c => c.Substring(6, 6)).Distinct().Count());
            }
            finally { File.Delete(e); File.Delete(s); }
        }
    }
}
