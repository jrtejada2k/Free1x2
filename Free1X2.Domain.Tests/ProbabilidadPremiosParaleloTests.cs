using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xunit;

namespace Free1X2.Domain.Tests
{
    /// <summary>
    /// #11: Probabilidad de premios por tramos en varios núcleos da lo mismo que el bucle de un hilo de
    /// antes (aquí <see cref="Referencia"/>). 100.000 columnas de madre son varios tramos (32.768 como
    /// mínimo cada uno).
    /// </summary>
    public class ProbabilidadPremiosParaleloTests
    {
        private static readonly int[] Pot = { 1, 3, 9, 27, 81, 243, 729, 2187, 6561, 19683, 59049, 177147, 531441, 1594323, 4782969 };

        [Theory]
        [InlineData(11)]
        [InlineData(13)]
        public void MismasProbabilidadesQueEnUnHilo(int minimo)
        {
            var hija = Columnas(50_000, semilla: 2);
            var madre = Columnas(100_000, semilla: 1);
            // Algunas columnas de la madre están en la hija (premio de 14 directo).
            for (int i = 0; i < madre.Count; i += 97) madre[i] = hija[i % hija.Count];
            string rutaMadre = Escribir(madre), rutaHija = Escribir(hija);
            try
            {
                var a = new Free1X2.Analisis.Analizador(minimo);
                a.ComparaCombinaciones(rutaMadre, rutaHija);

                double[] esperadas = Referencia(madre, hija, minimo);
                Assert.True(esperadas[14] > 0 && esperadas[minimo] > esperadas[14], "caso sin variedad");
                for (int c = 0; c <= 14; c++)
                    Assert.Equal(esperadas[c], a.ObtenProbabilidadPremios(c));
            }
            finally { File.Delete(rutaMadre); File.Delete(rutaHija); }
        }

        [Fact]
        public void ColumnaFueraDeRango_MismoError()
        {
            // 15 signos con el último «2»: el índice cae fuera del mapa de 14 partidos, como antes.
            var hija = Columnas(1_000, semilla: 4);
            var madre = Columnas(100_000, semilla: 3);
            madre[80_000] = "111111111111112";
            string rutaMadre = Escribir(madre), rutaHija = Escribir(hija);
            try
            {
                var a = new Free1X2.Analisis.Analizador(10);
                Assert.Throws<ArgumentOutOfRangeException>(() => a.ComparaCombinaciones(rutaMadre, rutaHija));
            }
            finally { File.Delete(rutaMadre); File.Delete(rutaHija); }
        }

        /// <summary>ComparaCombinaciones de un hilo de antes (Analisis/Analizador.cs, antes de #11).</summary>
        private static double[] Referencia(List<string> madre, List<string> hija, int minimo)
        {
            var bits = new bool[Pot[14]];
            foreach (string h in hija) bits[Numero(h)] = true;
            var directos = new int[17];
            var temp = new int[17];
            int profundidad = 0;
            void Distantes(int idx, int desde, int max)
            {
                profundidad++;
                for (int p = desde; p < 14; p++)
                {
                    int sig = idx / Pot[p] % 3;
                    for (int z = 0; z < 3; z++)
                    {
                        if (z == sig) continue;
                        int j = idx + Pot[p] * (z - sig);
                        if (bits[j]) temp[14 - profundidad]++;
                        else if (profundidad < max) Distantes(j, p + 1, max);
                    }
                }
                profundidad--;
            }
            foreach (string m in madre)
            {
                Array.Clear(temp, 0, temp.Length);
                int num = Numero(m);
                if (bits[num]) temp[14]++;
                else { profundidad = 0; Distantes(num, 0, 14 - minimo); }
                for (int i = 0; i < 17; i++) if (temp[i] != 0) directos[i]++;
            }
            var r = new double[17];
            for (int i = 0; i < 17; i++) r[i] = directos[i] * 100.0 / madre.Count;
            return r;
        }

        private static int Numero(string c)
        {
            int b = 0;
            for (int i = 0; i < c.Length; i++) b += c[i] == '1' ? 0 : c[i] == '2' ? 2 * Pot[i] : Pot[i];
            return b;
        }

        private static List<string> Columnas(int n, int semilla)
        {
            var r = new Random(semilla);
            var l = new List<string>(n);
            var sb = new StringBuilder();
            for (int i = 0; i < n; i++)
            {
                sb.Clear();
                for (int k = 0; k < 14; k++) sb.Append("1X2"[r.Next(3)]);
                l.Add(sb.ToString());
            }
            return l;
        }

        private static string Escribir(List<string> lineas)
        {
            string ruta = Path.Combine(Path.GetTempPath(), "free1x2_prob_" + Guid.NewGuid().ToString("N") + ".txt");
            File.WriteAllLines(ruta, lineas);
            return ruta;
        }
    }
}
